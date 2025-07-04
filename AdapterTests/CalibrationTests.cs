using Core.Configs;
using Infrastructure.Adapters.XInputs;
using Infrastructure.Services.Inputs;
using System.Reflection;
using Xunit;

namespace AdapterTests;

public class CalibrationTests
{
    [Fact]
    public void Adapter_Uses_Calibration_Values()
    {
        XInput.TestStateProvider = static () => new XInputInterop.XInputState
        {
            Gamepad = new XInputInterop.XInputGamepad
            {
                bLeftTrigger = 128,
                bRightTrigger = 255,
                sThumbLX = 32767,
                sThumbRX = -32768
            }
        };

        var adapter = new XInput();
        adapter.ApplyCalibration(new CalibrationSettings
        {
            LeftTriggerStart = 0.5f,
            LeftTriggerEnd = 1f,
            RightStickSensitivity = 0.5f,
            LeftStickDeadzoneInner = 0.2f,
            LeftStickDeadzoneOuter = 1f,
            RightStickDeadzoneInner = 0.2f,
            RightStickDeadzoneOuter = 1f
        });

        var state = adapter.GetState();

        Assert.InRange(state.TriggerLeft, 0.0f, 0.1f);
        Assert.Equal(1f, state.TriggerRight);
        Assert.Equal(1f, state.ThumbLX);
        Assert.Equal(-0.5f, state.ThumbRX, 3);
    }

    [Fact]
    public void Service_Forwards_Calibration_To_Adapter()
    {
        XInput.TestStateProvider = () => new XInputInterop.XInputState
        {
            Gamepad = new XInputInterop.XInputGamepad
            {
                sThumbLX = 32767
            }
        };

        var service = new XInputService();
        var settings = new CalibrationSettings { LeftStickSensitivity = 0.5f };
        service.UpdateCalibration(settings);

        var field = typeof(XInputService).GetField("_adapter", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var adapter = (XInput)field.GetValue(service)!;
        var state = adapter.GetState();

        Assert.Equal(0.5f, state.ThumbLX, 3);
    }
}