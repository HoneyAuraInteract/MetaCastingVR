using System;
using UnityEngine;
using BNG;
using System.Collections.Generic;

public class GrabObjectStep : ITrainingStep
{
    private Action onStepComplete;
    private bool completed = false;
   
    public void StartStep(Action onStepCompleted)
    {
        this.onStepComplete = onStepCompleted;
        completed = false;

        DebugCanvasLogger.LogStatic("[Step: GrabObject] Step started. Waiting for LEFT GRAB...");

        HighlightManager.Instance.ShowHighlight("LeftGrip");

        // Find all Grabbables in scene
        var grabbables = UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None);
        foreach (var grabbable in grabbables)
        {
            var events = grabbable.GetComponent<GrabbableUnityEvents>();

            if (events != null)
            {
                // Subscribe to the grab event once
                events.onGrab.AddListener(HandleGrab);
               
            }
            else
            {
                DebugCanvasLogger.LogStatic($"[Training] No GrabbableEvents on {grabbable.name}");
            }

        }
    }

    

    private void HandleGrab(Grabber obj)
    {
        DebugCanvasLogger.LogStatic("[Training] GrabObjectStep Completed");
        if (completed) return;

        if (obj != null && obj.HandSide == ControllerHand.Left)
        {
            completed = true;
            DebugCanvasLogger.LogStatic($"[Step: GrabObject] Successfully grabbed {obj.name} with LEFT HAND ✅");

            HighlightManager.Instance.HideHighlight("LeftGrip");

            // Cleanup listeners
            var grabbables = UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None);
            foreach (var grabbable in grabbables)
            {
                var events = grabbable.GetComponent<GrabbableUnityEvents>();

                if (events != null)
                {
                    // Subscribe to the grab event once
                    events.onGrab.RemoveAllListeners();

                }
             

            }

            onStepComplete?.Invoke();
        }
    }

    public string GetInstructionText()
    {
        return "Grab an object using the LEFT controller grip";
    }
}
