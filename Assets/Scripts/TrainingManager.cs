using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingManager : MonoBehaviour
{
    private Queue<ITrainingStep> trainingSteps;
    private ITrainingStep currentStep;

    [SerializeField] private TextMeshProUGUI instructionText;

    private void Start()
    {
        trainingSteps = new Queue<ITrainingStep>();

        // Add steps
        trainingSteps.Enqueue(new MoveForwardStep());
        trainingSteps.Enqueue(new MoveBackwardStep());
        trainingSteps.Enqueue(new MoveLeftStep());
        trainingSteps.Enqueue(new MoveRightStep());
       // trainingSteps.Enqueue(new PullTriggerStep());
        trainingSteps.Enqueue(new GrabObjectStep());
        trainingSteps.Enqueue(new GrabObjectRightHandStep());
        trainingSteps.Enqueue(new PullRightTriggerStep());
        trainingSteps.Enqueue(new RotateLeftSide());
        trainingSteps.Enqueue(new RotateRightSide());
        //trainingSteps.Enqueue(new PullTriggerStep());
        //trainingSteps.Enqueue(new GrabObjectStep());

        StartNextStep();
    }

    private void StartNextStep()
    {
        if (trainingSteps.Count == 0)
        {
            instructionText.text = "🎉 Training Complete!";
            return;
        }

        currentStep = trainingSteps.Dequeue();
        DebugCanvasLogger.LogStatic($"[Training] Starting Step: {currentStep.GetType().Name}");
        if (instructionText != null)
        instructionText.text = currentStep.GetInstructionText();

        // Let the step begin and subscribe to its completion event
        currentStep.StartStep(OnStepCompleted);
    }

    private void OnStepCompleted()
    {
        DebugCanvasLogger.LogStatic($"[Training] Completed Step: {currentStep.GetType().Name}");
        StartNextStep();
    }
}
