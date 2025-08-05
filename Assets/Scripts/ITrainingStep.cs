using System;

public interface ITrainingStep
{
    void StartStep(Action onStepCompleted); // Provide callback
    string GetInstructionText();            // Text to show
}

