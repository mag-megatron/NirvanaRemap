using Core.Entities;
using Infrastructure.Adapters.XInputs;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class GamepadInputTest
    {
        [Fact]
        public async Task Deve_Logar_Alteracoes_De_Input_Do_Gamepad()
        {
            var gamepad = new XInput();

            Console.WriteLine("Pronto. Pressione qualquer botão. Pressione ESC para encerrar.");

            using var cts = new CancellationTokenSource();

            await MonitorarInputsAsync(gamepad, cts.Token);

            Console.WriteLine("Encerrado.");
        }

        private async Task MonitorarInputsAsync(XInput gamepad, CancellationToken token)
        {
            var previousState = gamepad.GetState();

            while (!token.IsCancellationRequested)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;

                var current = gamepad.GetState();
                if (!current.Equals(previousState))
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Alteração detectada:");
                    Console.WriteLine(current);
                    previousState = current.Clone(); // Evita reuso da mesma instância
                }

                await Task.Delay(50, token);
            }
        }
    }
}
