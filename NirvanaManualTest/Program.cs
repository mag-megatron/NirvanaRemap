using System;
using System.Linq;
using System.Reflection;
using ApplicationLayer.Services;

using Core.Entities;
using Core.Services;
using Infrastructure.Adapters.Outputs;
using Infrastructure.Adapters.XInputs;

Console.WriteLine("🔧 Remapeamento de Gamepad iniciado.");

#region Preparação dos Inputs e Mapeamento Inicial

var inputs = typeof(GamepadState)
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Select(p => p.Name)
    .ToArray();

var mapping = inputs.ToDictionary(i => i, i => i); // Mapeamento 1:1 inicial
var profile = new ProfileMapping();

void BuildProfile()
{
    profile.ClearMappings();
    foreach (var pair in mapping)
        profile.AddMapping(new SimpleMapping(pair.Key, pair.Value));
}

void ShowCurrentMapping()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n📌 Mapeamento Atual:");
    foreach (var input in inputs)
        Console.WriteLine($"{input} -> {mapping[input]}");
    Console.ResetColor();
}

BuildProfile();
ShowCurrentMapping();

#endregion

#region Inicialização dos Serviços

var mappingService = new MappingService(profile);
var outputService = new ConsoleOutput();
var gamepadService = new XInputService();
var controller = new ControllerManager(gamepadService, mappingService, outputService, new CalibrationSettings());

controller.InputReceived += (_, input) =>
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[INPUT] {input.Name} = {input.Value:F2}");
    Console.ResetColor();
};

controller.StartListening();

#endregion

#region Loop de Interação do Usuário

while (true)
{
    Console.WriteLine("\n🎮 Selecione uma entrada do Gamepad para remapear (0 para sair):");

    for (int i = 0; i < inputs.Length; i++)
        Console.WriteLine($"{i + 1}. {inputs[i]} -> {mapping[inputs[i]]}");

    if (!int.TryParse(Console.ReadLine(), out var idx) || idx < 0 || idx > inputs.Length)
    {
        Console.WriteLine("❌ Opção inválida.\n");
        continue;
    }

    if (idx == 0) break;

    var selected = inputs[idx - 1];

    Console.WriteLine($"\n🔁 Nova entrada para [{selected}]:");
    for (int i = 0; i < inputs.Length; i++)
        Console.WriteLine($"{i + 1}. {inputs[i]}");

    if (!int.TryParse(Console.ReadLine(), out var idxNew) || idxNew < 1 || idxNew > inputs.Length)
    {
        Console.WriteLine("❌ Opção inválida.\n");
        continue;
    }

    var newInput = inputs[idxNew - 1];
    mapping[selected] = newInput;

    BuildProfile();
    Console.WriteLine("✅ Configuração atualizada.");
    ShowCurrentMapping();
}

#endregion

controller.StopListening();
Console.WriteLine("\nEncerrado.");
