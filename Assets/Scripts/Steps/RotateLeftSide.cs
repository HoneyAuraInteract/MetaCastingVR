using UnityEngine;
using BNG;
public class RotateLeftSide : HoldInputStepBase
{
    public override string GetInstructionText()
    {
        return "Push and hold the Right thumbstick RIGHT to rotate Left side";
    }

    public override bool IsInputConditionMet()
    {
        return InputBridge.Instance.RightThumbstickAxis.x > -0.6f;
    }

    protected override string GetHighlightKey()
    {
        return "RightJoystick";
    }
}
