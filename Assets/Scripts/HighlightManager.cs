using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance;

    [Header("References to highlights")]
    public GameObject leftJoystickHighlight;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowHighlight(string id)
    {
        if (id == "LeftJoystick" && leftJoystickHighlight != null)
            leftJoystickHighlight.SetActive(true);
    }

    public void HideHighlight(string id)
    {
        if (id == "LeftJoystick" && leftJoystickHighlight != null)
            leftJoystickHighlight.SetActive(false);
    }
}
