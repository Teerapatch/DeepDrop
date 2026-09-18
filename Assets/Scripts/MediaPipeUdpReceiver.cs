using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class MediaPipeUdpReceiver : MonoBehaviour
{
    [Header("UDP Settings")]
    public int port = 5052;
    public string expectedProtocolVersion = "1.0";

    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning;

    private MediaPipeData latestData;
    private readonly object dataLock = new object();
    
    private int lastProcessedSequence = -1;
    public float lastPacketTime { get; private set; }

    void Start()
    {
        StartReceiving();
    }

    private void StartReceiving()
    {
        try
        {
            udpClient = new UdpClient(port);
            isRunning = true;
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log($"[MediaPipeUdpReceiver] Listening on port {port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[MediaPipeUdpReceiver] Error starting UDP Client: {e.Message}");
        }
    }

    private void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref anyIP);
                string json = Encoding.UTF8.GetString(data);

                MediaPipeData parsedData = JsonUtility.FromJson<MediaPipeData>(json);

                if (parsedData.protocolVersion != expectedProtocolVersion)
                {
                    Debug.LogWarning($"[MediaPipeUdpReceiver] Protocol mismatch. Expected {expectedProtocolVersion}, got {parsedData.protocolVersion}");
                }

                lock (dataLock)
                {
                    latestData = parsedData;
                }
            }
            catch (SocketException)
            {
                // Normal when closing socket
            }
            catch (Exception e)
            {
                if (isRunning) Debug.LogError($"[MediaPipeUdpReceiver] Receive Error: {e.Message}");
            }
        }
    }

    void Update()
    {
        lock (dataLock)
        {
            if (latestData != null && latestData.sequenceNumber != lastProcessedSequence)
            {
                lastProcessedSequence = latestData.sequenceNumber;
                lastPacketTime = Time.time;
            }
        }
    }

    public MediaPipeData GetLatestData()
    {
        lock (dataLock)
        {
            return latestData;
        }
    }

    void OnDisable() { StopReceiving(); }
    void OnDestroy() { StopReceiving(); }
    void OnApplicationQuit() { StopReceiving(); }

    private void StopReceiving()
    {
        if (!isRunning) return;
        isRunning = false;
        
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }
        
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(500);
            if (receiveThread.IsAlive)
            {
                receiveThread.Abort();
            }
        }
    }
}
