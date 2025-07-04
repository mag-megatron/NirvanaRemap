// Patch: Correção de detecção de gamepad físico + tolerância a falhas COM
using ApplicationLayer.Services;
using Core.Entities;
using Core.Events.Inputs;
using Core.Events.Outputs;
using Core.Interfaces;
using Infrastructure.Adapters.Outputs;
using NirvanaMAUI.Models;
using System.Runtime.InteropServices;
using Windows.Gaming.Input;

namespace NirvanaMAUI.Services
{
    public class GamepadRemapService
    {
        private Gamepad? _currentGamepad;
        private Profile? _activeProfile;
        private CancellationTokenSource? _pollingCts;
        private readonly IOutputService _output;
        private MappingService? _mappingService;
        public event Action<string, float>? InputReceived;
        public event Action<string, float>? OutputApplied;
        public event Action<bool>? GamepadConnectionChanged;

        private readonly Dictionary<string, float> _virtualState = new();

        private readonly Dictionary<string, float> _currentInputs = new();
        private readonly string[] AllOutputNames = new[]
            {
                "ButtonA", "ButtonB", "ButtonX", "ButtonY",
                "DPadUp", "DPadDown", "DPadLeft", "DPadRight",
                "ButtonStart", "ButtonBack", "ButtonLeftShoulder", "ButtonRightShoulder",
                "ThumbLPressed", "ThumbRPressed", "TriggerLeft", "TriggerRight",
                "ThumbLX", "ThumbLY", "ThumbRX", "ThumbRY"
            };

        public bool IsGamepadConnected => _currentGamepad != null;

        public GamepadRemapService()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _output = new ViGEmOutput();
            else
                _output = new ConsoleOutput();
            Gamepad.GamepadAdded += OnGamepadAdded;
            Gamepad.GamepadRemoved += OnGamepadRemoved;

            _currentGamepad = Gamepad.Gamepads.FirstOrDefault(IsRealGamepad);
            GamepadConnectionChanged?.Invoke(_currentGamepad != null);
        }

        private bool IsRealGamepad(Gamepad pad)
        {
            if (pad == null) return false;

            var reading = pad.GetCurrentReading();

            // Check if the reading is not default (all zeroes).
            // Real gamepads tendem a ter triggers, thumbsticks ou botões diferentes de zero.
            return reading.LeftTrigger != 0 ||
                   reading.RightTrigger != 0 ||
                   reading.LeftThumbstickX != 0 ||
                   reading.LeftThumbstickY != 0 ||
                   reading.RightThumbstickX != 0 ||
                   reading.RightThumbstickY != 0 ||
                   reading.Buttons != GamepadButtons.None;
        }

        public void Start(Profile profile)
        {
            _activeProfile = profile;
            _mappingService = new MappingService(profile.ToProfileMapping());

            if (_currentGamepad == null && Gamepad.Gamepads.Count > 0)
                _currentGamepad = Gamepad.Gamepads[0];

            _output.Connect(); // ← AQUI!

            _pollingCts?.Cancel();
            _pollingCts = new CancellationTokenSource();
            _ = PollGamepadAsync(_pollingCts.Token);

            GamepadConnectionChanged?.Invoke(_currentGamepad != null);
        }


        public void Stop()
        {
            _pollingCts?.Cancel();
            _pollingCts = null;
            _currentGamepad = null;
            _output.Disconnect();

            GamepadConnectionChanged?.Invoke(false);
        }

        public void Restart()
        {
            if (_activeProfile != null)
                Start(_activeProfile);
        }

        private async Task PollGamepadAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_currentGamepad == null || !Gamepad.Gamepads.Contains(_currentGamepad) || !IsRealGamepad(_currentGamepad))
                {
                    _currentGamepad = Gamepad.Gamepads.FirstOrDefault(IsRealGamepad);
                    GamepadConnectionChanged?.Invoke(_currentGamepad != null);
                    if (_currentGamepad == null)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                }

                var reading = _currentGamepad.GetCurrentReading();

                System.Diagnostics.Debug.WriteLine($"ThumbLX: {reading.LeftThumbstickX}, ThumbLY: {reading.LeftThumbstickY}, ThumbRX: {reading.RightThumbstickX}, ThumbRY: {reading.RightThumbstickY}");
                ReportInputs(reading);
                SyncVirtualController();
                await Task.Delay(20); // ~50Hz
            }
        }

        private void SyncVirtualController()
        {
            // Zere todos os outputs por padrão
            foreach (var output in AllOutputNames)
                _virtualState[output] = 0f;

            // Para cada input, aplique o mapping (pode haver mais de um para o mesmo output)
            foreach (var kvp in _currentInputs)
            {
                var input = new ControllerInput(kvp.Key, kvp.Value);
                var mapped = _mappingService.Map(input);

                foreach (var output in mapped)
                {
                    // Se dois inputs físicos estiverem mapeando para o mesmo output virtual, use o maior valor
                    if (_virtualState.ContainsKey(output.OutputName))
                        _virtualState[output.OutputName] = Math.Max(_virtualState[output.OutputName], output.Value);
                    else
                        _virtualState[output.OutputName] = output.Value;
                }
            }
           
            _output.ApplyAll(_virtualState);
            // Envie TODO o estado ao ViGEm
            foreach (var output in _virtualState)
            {          
                OutputApplied?.Invoke(output.Key, output.Value);
            }
        }


        private void ReportInputs(GamepadReading reading)
        {
            Emit("ButtonA", reading.Buttons.HasFlag(GamepadButtons.A));
            Emit("ButtonB", reading.Buttons.HasFlag(GamepadButtons.B));
            Emit("ButtonX", reading.Buttons.HasFlag(GamepadButtons.X));
            Emit("ButtonY", reading.Buttons.HasFlag(GamepadButtons.Y));
            Emit("ButtonLeftShoulder", reading.Buttons.HasFlag(GamepadButtons.LeftShoulder));
            Emit("ButtonRightShoulder", reading.Buttons.HasFlag(GamepadButtons.RightShoulder));
            Emit("ButtonBack", reading.Buttons.HasFlag(GamepadButtons.View));
            Emit("ButtonStart", reading.Buttons.HasFlag(GamepadButtons.Menu));
            Emit("DPadLeft", reading.Buttons.HasFlag(GamepadButtons.DPadLeft));
            Emit("DPadRight", reading.Buttons.HasFlag(GamepadButtons.DPadRight));
            Emit("DPadUp", reading.Buttons.HasFlag(GamepadButtons.DPadUp));
            Emit("DPadDown", reading.Buttons.HasFlag(GamepadButtons.DPadDown));
            Emit("ThumbLPressed", reading.Buttons.HasFlag(GamepadButtons.LeftThumbstick));
            Emit("ThumbRPressed", reading.Buttons.HasFlag(GamepadButtons.RightThumbstick));

            Emit("TriggerLeft", (float)reading.LeftTrigger);
            Emit("TriggerRight", (float)reading.RightTrigger);

            Emit("ThumbLX", (float)reading.LeftThumbstickX);
            Emit("ThumbLY", (float)reading.LeftThumbstickY);
            Emit("ThumbRX", (float)reading.RightThumbstickX);
            Emit("ThumbRY", (float)reading.RightThumbstickY);
        }

        private void Emit(string name, bool state) => Emit(name, state ? 1f : 0f);

        private void Emit(string name, float value)
        {
            _currentInputs[name] = value;

            InputReceived?.Invoke(name, value);
            if (_mappingService != null)
            {
                var input = new ControllerInput(name, value);
                var mapped = _mappingService.Map(input);
                foreach (var output in mapped)
                {
                    OutputApplied?.Invoke(output.OutputName, output.Value);
                }
            }
        }

        private void OnGamepadAdded(object sender, Gamepad e)
        {
            if (_currentGamepad == null && IsRealGamepad(e))
            {
                _currentGamepad = e;
                GamepadConnectionChanged?.Invoke(true);

                if (_activeProfile != null)
                {
                    _pollingCts?.Cancel();
                    _pollingCts = new CancellationTokenSource();
                    _ = PollGamepadAsync(_pollingCts.Token);
                }
            }
        }

        private void OnGamepadRemoved(object sender, Gamepad e)
        {
            if (_currentGamepad == e)
            {
                _currentGamepad = null;
                GamepadConnectionChanged?.Invoke(false);
                _pollingCts?.Cancel();
            }
        }
    }
}
