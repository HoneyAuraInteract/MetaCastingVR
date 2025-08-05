using System;
using UnityEngine;
using BNG;

public class MoveBackwardStep : HoldInputStepBase
{
    
        public override bool IsInputConditionMet()
        {
            return InputBridge.Instance.LeftThumbstickAxis.y < -0.6f;
        }

        public override string GetInstructionText()
        {
            return "Push and hold the LEFT thumbstick BACKWARD to move backwards";
        }

        protected override string GetHighlightKey()
        {
            return "LeftThumbstick";
        }
    

}
