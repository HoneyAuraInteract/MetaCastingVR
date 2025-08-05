using System;
using UnityEngine;
using BNG;

public abstract class HoldInputStepBase : ITrainingStep
{
    protected Action onStepComplete;
    protected bool completed = false;
    protected float holdTime = 2.0f;
    protected float timer = 0f;

    public void StartStep(Action onStepCompleted)
    {
        this.onStepComplete = onStepCompleted;
        completed = false;
        timer = 0f;

        Debug.Log($"[Step: {GetType().Name}] Started...");
        HighlightManager.Instance.ShowHighlight(GetHighlightKey());

        InputEvents.OnInputUpdated += CheckInput; 
    }

    private void CheckInput()
    {
        if (completed) return;

        if (IsInputConditionMet())
        {
            timer += Time.deltaTime;
            if (timer >= holdTime)
            {
                CompleteStep();
            }
        }
        else
        {
            timer = 0f;
        }
    }

    private void CompleteStep()
    {
        completed = true;
        InputEvents.OnInputUpdated -= CheckInput;
        HighlightManager.Instance.HideHighlight(GetHighlightKey());

        Debug.Log($"[Step: {GetType().Name}] Completed ✅");
        onStepComplete?.Invoke();
    }

    public abstract bool IsInputConditionMet();
    public abstract string GetInstructionText();
    protected abstract string GetHighlightKey();
}
