using QuickOutline;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FlashState { Disable, Enable }

public class FlashingOutline : MonoBehaviour
{
    #region Variable
    //--------------------------------------------------------------//



    [SerializeField] public bool startOnAwake;
    Outline outline;


    private bool flashingOutline;

    //--------------------------------------------------------------//
    #endregion




    #region Unity Method
    //--------------------------------------------------------------//

    private void OnEnable()
    {
        OnFlashStateChanged += FlashingOutline_OnFlashStateChanged;
        outline = GetComponent<Outline>();
        if (startOnAwake)
        {
            StartFlashing();
        }
    }


    private void OnDisable()
    {
        OnFlashStateChanged -= FlashingOutline_OnFlashStateChanged;
        StopAllCoroutines();
        if (co != null) StopCoroutine(co);
    }

    //--------------------------------------------------------------//
    #endregion



    Coroutine co;

    public event Action OnFlashStateChanged;
    [SerializeField] private FlashState _currentState;
    public FlashState CurrentState { get => _currentState; set { _currentState = value; OnFlashStateChanged?.Invoke(); } }
    #region Public
    //--------------------------------------------------------------//

    public void StartFlashing()
    {
         co = StartCoroutine(Flashing());
    }
    
    public void StopFlashing()
    {
        flashingOutline = false;
        if (co != null) StopCoroutine(co);
        outline.enabled = false;
    }

    private void FlashingOutline_OnFlashStateChanged()
    {
        //Debug.Log(CurrentState);
        switch (CurrentState)
        {
            case FlashState.Disable:
                StopFlashing();
                break;
            case FlashState.Enable:
                StartFlashing();
                break;
            default:
                break;
        }
    }

    //--------------------------------------------------------------//
    #endregion



    #region Private
    //--------------------------------------------------------------//

    IEnumerator Flashing()
    {
        flashingOutline = true;

        while (flashingOutline)
        {
            yield return new WaitForSeconds(0.5f);
            outline.enabled = true;
            yield return new WaitForSeconds(0.5f);
            outline.enabled = false;
        }
    }

    public static implicit operator FlashingOutline(FlashState v)
    {
        throw new NotImplementedException();
    }

    //--------------------------------------------------------------//
    #endregion
}
