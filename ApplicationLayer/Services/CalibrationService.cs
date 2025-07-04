using Core.Entities;
using Core.Interfaces;

namespace ApplicationLayer.Services;

public class CalibrationService : ICalibrationService
{
    public float ApplyDeadzone(float input, float deadzone)
    {
        return Math.Abs(input) < deadzone ? 0 : input;
    }

    public float AdjustSensitivity(float input, float sensitivity)
    {
        return Math.Clamp(input * sensitivity, -1.0f, 1.0f);
    }

    public GamepadState Calibrate(GamepadState rawState, CalibrationSettings settings)
    {
        return new GamepadState
        {
            ThumbLX = AdjustSensitivity(ApplyDeadzone(rawState.ThumbLX, settings.LeftStickDeadzoneInner), settings.LeftStickSensitivity),
            ThumbLY = AdjustSensitivity(ApplyDeadzone(rawState.ThumbLY, settings.LeftStickDeadzoneInner), settings.LeftStickSensitivity),
            ThumbRX = AdjustSensitivity(ApplyDeadzone(rawState.ThumbRX, settings.RightStickDeadzoneInner), settings.RightStickSensitivity),
            ThumbRY = AdjustSensitivity(ApplyDeadzone(rawState.ThumbRY, settings.RightStickDeadzoneInner), settings.RightStickSensitivity),
            TriggerLeft = AdjustSensitivity(rawState.TriggerLeft, 1f),
            TriggerRight = AdjustSensitivity(rawState.TriggerRight, 1f),

            ButtonA = rawState.ButtonA,
            ButtonB = rawState.ButtonB,
            ButtonX = rawState.ButtonX,
            ButtonY = rawState.ButtonY,
            DPadUp = rawState.DPadUp,
            DPadDown = rawState.DPadDown,
            DPadLeft = rawState.DPadLeft,
            DPadRight = rawState.DPadRight,
            ButtonStart = rawState.ButtonStart,
            ButtonBack = rawState.ButtonBack,
            ButtonLeftShoulder = rawState.ButtonLeftShoulder,
            ButtonRightShoulder = rawState.ButtonRightShoulder,
            ThumbLPressed = rawState.ThumbLPressed,
            ThumbRPressed = rawState.ThumbRPressed
        };
    }
}