using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvasLogger : MonoBehaviour
{
    public static DebugCanvasLogger Instance;

    [Header("UI References")]
    [SerializeField] private Transform contentParent;       // Assign: ScrollView > Viewport > Content
    [SerializeField] private ScrollRect scrollRect;         // Assign the ScrollRect component
    [SerializeField] private GameObject debugTextPrefab;    // Assign the TMP Text prefab

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static void LogStatic(string message)
    {
        Instance?.Log(message);
    }

    public void Log(string message)
    {
        GameObject newLog = Instantiate(debugTextPrefab, contentParent);
        TMP_Text logText = newLog.GetComponent<TMP_Text>();
        logText.text = message;

        // Force scroll to bottom
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public static void LogStaticError(string message)
    {
        Instance?.Log(message);
    }


}
