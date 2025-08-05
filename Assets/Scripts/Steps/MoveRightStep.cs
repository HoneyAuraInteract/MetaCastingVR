using System;
using UnityEngine;
using BNG;

public class MoveRightStep : HoldInputStepBase
{
    public override bool IsInputConditionMet()
    {
        return InputBridge.Instance.LeftThumbstickAxis.x > 0.6f;
    }

    public override string GetInstructionText()
    {
        return "Push and hold the LEFT thumbstick RIGHT to move right side";
    }

    protected override string GetHighlightKey()
    {
        return "LeftThumbstick";
    }
}
