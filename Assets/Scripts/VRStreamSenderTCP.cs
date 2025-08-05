using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using UnityEngine.Rendering; // Required for AsyncGPUReadback
using System.Linq;           // Needed for request.GetData<byte>().ToArray()
public class VRStreamSenderTCP : MonoBehaviour
{
    private RenderTexture sourceTexture;
    public int textureWidth = 1024;
    public int textureHeight = 1024;

    public string receiverIP = "192.168.1.5"; // Replace with Mac IP
    public int port = 9999;

    private TcpClient client;
    private NetworkStream stream;
    private Texture2D readableTexture;
    public Camera captureCamera; // Assign your VR camera here

    void Start()
    {
        sourceTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);
        sourceTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt;

        sourceTexture.Create();
        if (captureCamera != null)
        {
            captureCamera.targetTexture = sourceTexture;
        }

        Debug.Log("[Sender] Connecting to receiver at " + receiverIP + ":" + port);
        client = new TcpClient();

        try
        {
            client.Connect(receiverIP, port);
            stream = client.GetStream();
            Debug.Log("[Sender] Connected to receiver successfully.");

            readableTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGB24, false);
            InvokeRepeating(nameof(CaptureAndSendFrame), 0.2f, 0.1f); // Start after 0.2s, repeat every 0.1s (10 fps)

        }
        catch (SocketException ex)
        {
            Debug.LogError("[Sender] Connection failed: " + ex.Message);
        }
    }
    void CaptureAndSendFrame()
    {
        AsyncGPUReadback.Request(sourceTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.LogError("[Sender] GPU readback error!");
            return;
        }

        byte[] rawData = request.GetData<byte>().ToArray();

        // Convert to Texture2D
        readableTexture.LoadRawTextureData(rawData);
        readableTexture.Apply();

        byte[] imageBytes = readableTexture.EncodeToJPG(); // ✅ COMPRESSED JPG
        byte[] lengthPrefix = System.BitConverter.GetBytes(imageBytes.Length);

        try
        {
            stream.Write(lengthPrefix, 0, 4);
            stream.Write(imageBytes, 0, imageBytes.Length);
            Debug.Log("[Sender] Frame sent. Size: " + imageBytes.Length);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[Sender] Failed to send frame: " + ex.Message);
        }
    }



    IEnumerator SendFrames()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f); // 10 fps

        while (true)
        {
            yield return delay;

            RenderTexture.active = sourceTexture;
            readableTexture.ReadPixels(new Rect(0, 0, sourceTexture.width, sourceTexture.height), 0, 0);
            readableTexture.Apply();

            byte[] imageBytes = readableTexture.EncodeToJPG();
            byte[] lengthPrefix = System.BitConverter.GetBytes(imageBytes.Length);

            try
            {
                stream.Write(lengthPrefix, 0, 4);
                stream.Write(imageBytes, 0, imageBytes.Length);
                Debug.Log("[Sender] Frame sent. Size: " + imageBytes.Length);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[Sender] Failed to send frame: " + ex.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
    }
}
