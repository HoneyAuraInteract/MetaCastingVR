using System;
using UnityEngine;
using BNG; // Make sure you’re using the BNG namespace

public class MoveForwardStep : HoldInputStepBase
{
    public override bool IsInputConditionMet()
    {
        return InputBridge.Instance.LeftThumbstickAxis.y > 0.6f;
    }

    public override string GetInstructionText()
    {
        return "Push and hold the LEFT thumbstick FORWARD to move forward";
    }

    protected override string GetHighlightKey()
    {
        return "LeftThumbstick";
    }
}
