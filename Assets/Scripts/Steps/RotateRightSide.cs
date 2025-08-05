using UnityEngine;
using BNG;
public class RotateRightSide : HoldInputStepBase
{
    public override string GetInstructionText()
    {
        return "Push and hold the Right thumbstick RIGHT to rotate Right side";
    }

    public override bool IsInputConditionMet()
    {
        return InputBridge.Instance.RightThumbstickAxis.x > 0.6f;
    }

    protected override string GetHighlightKey()
    {
        return "RightJoystick";
    }
}
