using System;
using BNG;
using UnityEngine;



public class GrabObjectRightHandStep : ITrainingStep
{
    private Action onStepComplete;
    private bool completed = false;

    public void StartStep(Action onStepCompleted)
    {
        this.onStepComplete = onStepCompleted;
        completed = false;

        Debug.Log("[Step: GrabRightHand] Step started. Waiting for RIGHT GRAB...");

        HighlightManager.Instance.ShowHighlight("RightGrip");

        var grabbables = UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None);
        foreach (var grabbable in grabbables)
        {
            var events = grabbable.GetComponent<GrabbableUnityEvents>();

            if (events != null)
            {
                events.onGrab.AddListener(HandleGrab);
            }
            else
            {
                Debug.LogWarning($"[Training] No GrabbableUnityEvents on {grabbable.name}");
            }
        }
    }

    private void HandleGrab(Grabber grabber)
    {
        if (completed) return;

        if (grabber != null && grabber.HandSide == ControllerHand.Right)
        {
            completed = true;
            Debug.Log($"[Step: GrabRightHand] Successfully grabbed object with RIGHT HAND ✅");

            HighlightManager.Instance.HideHighlight("RightGrip");

            var grabbables = UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None);
            foreach (var grabbable in grabbables)
            {
                var events = grabbable.GetComponent<GrabbableUnityEvents>();

                if (events != null)
                {
                    events.onGrab.RemoveAllListeners();
                }
            }

            onStepComplete?.Invoke();
        }
    }

    public string GetInstructionText()
    {
        return "Grab an object using the RIGHT controller grip";
    }
}
