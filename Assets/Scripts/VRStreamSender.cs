using UnityEngine;
using Unity.WebRTC;
using System.Collections;

public class VRStreamSender : MonoBehaviour
{
    public RenderTexture targetRenderTexture;

    private RTCPeerConnection peer;
    private VideoStreamTrack videoTrack;
    private Coroutine webRtcUpdateCoroutine;
    public WebSocketSignaler signaler;

    void OnEnable()
    {
        Debug.Log("OnEnale");
        signaler.OnConnected += StartSetUp;
    }

    void Start()
    {
        Debug.Log("Start");

        // Start WebRTC update loop
        webRtcUpdateCoroutine = StartCoroutine(WebRTC.Update());
        signaler.OnMessageReceived += OnMessageReceived;

       

        // Begin sender setup
        
    }

    void StartSetUp()
    {
        DebugCanvasLogger.LogStatic(" Starting SetUpSender (after WebSocket connected)");

        StartCoroutine(SetUpSender());
      

    }
    void OnMessageReceived(string json)
    {
        // Handle answer
        SignalMessage msg = JsonUtility.FromJson<SignalMessage>(json);
        if (msg.type == "answer")
        {
            DebugCanvasLogger.LogStatic("Got answer SDP: " + msg.sdp);
            OnReceiveAnswer(msg.sdp);
            // Pass to WebRTC system here
        }
        if (msg.type == "candidate")
        {
            RTCIceCandidateInit candidateInit = new RTCIceCandidateInit
            {
                candidate = msg.candidate,
                sdpMid = msg.sdpMid,
                sdpMLineIndex = msg.sdpMLineIndex
            };

            var candidate = new RTCIceCandidate(candidateInit);
            peer.AddIceCandidate(candidate);
            DebugCanvasLogger.LogStatic("📥 Received and added ICE candidate.");
        }

    }
    private IEnumerator SetUpSender()
    {
        // Wait until RenderTexture is initialized
        Debug.Log("⏳ Waiting for RenderTexture to be ready...");

        yield return new WaitUntil(() => targetRenderTexture != null && targetRenderTexture.width > 0);
        Debug.Log("✅ RenderTexture is ready!");

        // Create Peer Connection
        peer = new RTCPeerConnection();
        peer.OnIceCandidate = candidate =>
        {
            SignalMessage msg = new SignalMessage
            {
                type = "candidate",
                candidate = candidate.Candidate,
                sdpMid = candidate.SdpMid,
                sdpMLineIndex = candidate.SdpMLineIndex ?? 0
            };

            string json = JsonUtility.ToJson(msg);
            signaler.SendMessage(json);
            DebugCanvasLogger.LogStatic("📤 Sent ICE candidate.");
        };

        // Create Video Track from the RenderTexture
        videoTrack = new VideoStreamTrack(targetRenderTexture);
        DebugCanvasLogger.LogStatic("✅ Video track created from RenderTexture.");  // ✅ ADD THIS
        DebugCanvasLogger.LogStatic($"🎥 Sender RenderTexture status: isCreated={targetRenderTexture.IsCreated()}, size={targetRenderTexture.width}x{targetRenderTexture.height}, format={targetRenderTexture.format}");

        peer.AddTrack(videoTrack);
        DebugCanvasLogger.LogStatic("📡 Video track added to peer connection.");    // ✅ ADD THIS


        // Create Offer
        var offerOp = peer.CreateOffer();
        yield return offerOp;

        if (offerOp.IsError)
        {
            DebugCanvasLogger.LogStaticError($"Offer creation failed: {offerOp.Error.message}");
            yield break;
        }

        var desc = offerOp.Desc;

        // Set Local Description
        var descOp = peer.SetLocalDescription(ref desc);
        yield return descOp;

        if (descOp.IsError)
        {
            DebugCanvasLogger.LogStaticError($"SetLocalDescription failed: {descOp.Error.message}");
        }
        else
        {
            DebugCanvasLogger.LogStatic("Local SDP offer:\n" + desc.sdp);
            SignalMessage offer = new SignalMessage()
            {
                type = "offer",
                sdp = desc.sdp
            };

            string json = JsonUtility.ToJson(offer);
            signaler.SendMessage(json);
            DebugCanvasLogger.LogStatic("📤 Sending Offer SDP:\n" + offer.sdp);

        }

        // Optional: Send `desc.sdp` to your signaling server here
    }
    void Update()
    {
        if (targetRenderTexture != null)
        {
            Graphics.Blit(null, targetRenderTexture); // or render some dynamic content
        }
    }

    public void OnReceiveAnswer(string sdpAnswer)
    {
        var answerDesc = new RTCSessionDescription
        {
            type = RTCSdpType.Answer,
            sdp = sdpAnswer
        };

        StartCoroutine(SetRemoteDescription(peer, answerDesc));

    }
    IEnumerator SetRemoteDescription(RTCPeerConnection peer, RTCSessionDescription desc)
    {
        var op = peer.SetRemoteDescription(ref desc);
        yield return op;

        if (op.IsError)
        {
            DebugCanvasLogger.LogStaticError("SetRemoteDescription failed: " + op.Error.message);
        }
        else
        {
            DebugCanvasLogger.LogStatic("Remote description set successfully.");
        }
    }


    void OnDestroy()
    {
        // Cleanup
        if (webRtcUpdateCoroutine != null)
            StopCoroutine(webRtcUpdateCoroutine);

        videoTrack?.Dispose();
        peer?.Close();
        peer?.Dispose();
        signaler.OnMessageReceived -= OnMessageReceived;

    }
}
[System.Serializable]
public class SignalMessage
{
    public string type;
    public string sdp;
    public string candidate;
    public string sdpMid;
    public int sdpMLineIndex;
}
