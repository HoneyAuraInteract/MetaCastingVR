using System;
using UnityEngine;
using BNG;

public class MoveLeftStep : HoldInputStepBase
{
    public override bool IsInputConditionMet()
    {
        return InputBridge.Instance.LeftThumbstickAxis.x < -0.6f;
    }

    public override string GetInstructionText()
    {
        return "Push and hold the LEFT thumbstick LEFT to move left side";
    }

    protected override string GetHighlightKey()
    {
        return "LeftThumbstick";
    }
}
