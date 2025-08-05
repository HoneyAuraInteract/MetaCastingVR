using System;
using UnityEngine;
using BNG;

public class PullRightTriggerStep : HoldInputStepBase
{
    public override bool IsInputConditionMet()
    {
        return InputBridge.Instance.RightTrigger > 0.8f;
    }

    protected override string GetHighlightKey()
    {
        return "RightTrigger";
    }

    public override string GetInstructionText()
    {
        return "Press and hold the Right trigger";
    }
}
