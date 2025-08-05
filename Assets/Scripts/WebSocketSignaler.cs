using UnityEngine;
using NativeWebSocket;
using System;
using System.Text;
using System.Threading.Tasks;

public class WebSocketSignaler : MonoBehaviour
{
    private WebSocket websocket;

    public Action<string> OnMessageReceived;
    private TaskCompletionSource<bool> connectionTcs = new TaskCompletionSource<bool>();
    public event Action OnConnected;

    private async void Start()
    {
        websocket = new WebSocket("ws://192.168.1.207:8080");

        websocket.OnOpen += () =>
        {
            DebugCanvasLogger.LogStatic("WebSocket connected!");
            OnConnected?.Invoke();
        };

        websocket.OnError += (e) =>
        {
            DebugCanvasLogger.LogStaticError("WebSocket Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            DebugCanvasLogger.LogStatic("WebSocket closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            DebugCanvasLogger.LogStatic("Received message: " + message);
            OnMessageReceived?.Invoke(message);
        };

        await websocket.Connect();
    }

    public async void SendMessage(string message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(message);
        }
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnDestroy()
    {
        await websocket.Close();
    }


    public async Task WaitForConnection()
    {
        await connectionTcs.Task;
    }

}
