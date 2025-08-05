using UnityEngine;
using System;

public class InputEvents : MonoBehaviour
{
    public static event Action OnInputUpdated;

    private void Update()
    {
        OnInputUpdated?.Invoke();
    }
}
