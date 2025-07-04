// Para IGamepadService, IOutputService

using Core.Entities;
using Core.Events.Inputs;
using Core.Events.Outputs;
using Core.Interfaces;
using Infrastructure.Adapters.Outputs;
using Infrastructure.Adapters.XInputs; // Para ControllerInput
// using Core.Events.Outputs; // Não é diretamente usado aqui, mas sim via IOutputService e MappingService

namespace ApplicationLayer.Services;

/// <summary>
/// Orquestra o fluxo de entrada, mapeamento e saída de comandos do controlador.
/// Conecta o serviço de entrada do gamepad, o serviço de mapeamento e o serviço de saída.
/// </summary>
/// <param name="inputService">O serviço para obter entradas do gamepad.</param>
/// <param name="mappingService">O serviço para processar e mapear as entradas.</param>
/// <param name="outputService">O serviço para aplicar as saídas mapeadas.</param>
public class ControllerManager
{
    private readonly IGamepadService _gamepadService;
    private readonly IMappingService _mappingService;
    private readonly IOutputService _outputService;
    private CalibrationSettings _calibrationSettings;
    public event Action<GamepadState>? RawStateReceived;


    public ControllerManager(
        IGamepadService inputService,
        IMappingService mappingService,
        IOutputService outputService, 
        CalibrationSettings calibrationSettings)
    {
        _gamepadService = inputService;
        _mappingService = mappingService;
        _outputService = outputService;
        _calibrationSettings = calibrationSettings;
    }

    public event EventHandler<ControllerInput>? InputReceived;
    /// <summary>
    /// Inicia o processo de escuta e mapeamento de entradas do controlador.
    /// Subscreve aos eventos de entrada do <see cref="IGamepadService"/> e inicia a sua escuta.
    /// </summary>
    public void StartListening()
    {
        _gamepadService.InputReceived -= OnInputReceived; // Remove antes de adicionar
        _gamepadService.StateChanged += OnRawState;
        _gamepadService.InputReceived += OnInputReceived;
        _gamepadService.StartListening();
    }

    /// <summary>
    /// Para o processo de escuta e mapeamento de entradas do controlador.
    /// Cancela a subscrição aos eventos de entrada do <see cref="IGamepadService"/> e para a sua escuta.
    /// </summary>
    public void StopListening()
    {
        _gamepadService.StopListening();
        _gamepadService.StateChanged -= OnRawState;
        _gamepadService.InputReceived -= OnInputReceived;
        
    }
    private void OnRawState(object? sender, GamepadState state)
    {
        RawStateReceived?.Invoke(state);
    }

    /// <summary>
    /// Manipulador de evento chamado quando uma <see cref="ControllerInput"/> é recebida
    /// do <see cref="IGamepadService"/>.
    /// Processa a entrada através do <see cref="MappingService"/> e aplica cada
    /// <see cref="Core.Events.Outputs.MappedOutput"/> resultante através do <see cref="IOutputService"/>.
    /// </summary>
    private void OnInputReceived(object? sender, ControllerInput input)
    {
        var mappedOutputs = _mappingService.Map(input);

        foreach (var output in mappedOutputs)
        {
            _outputService.Apply(output);
        }
        InputReceived?.Invoke(this, input);
    }

    /// <summary>
    /// Aplica as configurações de calibração em tempo real ao gamepad.
    /// </summary>
    public void ApplyCalibration(CalibrationSettings settings)
    {
        _gamepadService?.UpdateCalibration(settings);
    }

    /// <summary>
    /// Apenas armazena localmente (opcional, pode remover se não usar).
    /// </summary>
    public void SetCalibration(CalibrationSettings settings)
    {
        _calibrationSettings = settings;
    }
}