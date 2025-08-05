using System;
using UnityEngine;
using BNG;

public class PullTriggerStep : HoldInputStepBase
{
    public override bool IsInputConditionMet()
    {
        return InputBridge.Instance.LeftTrigger > 0.8f;
    }

    protected override string GetHighlightKey()
    {
        return "LeftTrigger";
    }

    public override string GetInstructionText()
    {
        return "Press LEFT trigger";
    }
}
