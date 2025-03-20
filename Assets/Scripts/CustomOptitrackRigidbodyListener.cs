using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Linq;

public class CustomOptitrackRigidbodyListener : MonoBehaviour
{
    [Header("Optitrack Settings")]
    [Tooltip("The ID of the rigid body to track (set in Motive)")]
    public int rigidBodyID;
    
    [Tooltip("IP address of the computer running Motive")]
    public string serverIP = "127.0.0.1";
    
    [Tooltip("Port number for receiving data (default: 1511)")]
    public int dataPort = 1511;

    private Socket dataSocket;
    private bool isConnected = false;
    private byte[] receiveBuffer = new byte[65535];
    private bool isInitialized = false;
    private float lastDataTime = 0f;
    private float connectionCheckInterval = 5f; // Check connection every 5 seconds
    private EndPoint remoteEndPoint;
    private IPEndPoint serverEndPoint;

    // NatNet data structures
    [StructLayout(LayoutKind.Sequential)]
    private struct sRigidBodyData
    {
        public int ID;
        public float x, y, z;
        public float qx, qy, qz, qw;
        public float error;
        public int parameters;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct sFrameOfMocapData
    {
        public int iFrame;
        public int nMarkerSets;
        public int nOtherMarkers;
        public int nRigidBodies;
        public int nSkeletons;
        public int nLabeledMarkers;
        public int nForcePlates;
        public int nDevices;
        public double fTimestamp;
        public int CameraMidExposure;
        public int CameraDataReceived;
        public double fTransmitTimestamp;
        public int parameters;
        public int bRecording;
        public int trackedModelsChanged;
    }

    private void Start()
    {
        LogNetworkInterfaces();
        InitializeNatNet();
    }

    private void LogNetworkInterfaces()
    {
        Debug.Log("Available Network Interfaces:");
        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface ni in interfaces)
        {
            if (ni.OperationalStatus == OperationalStatus.Up)
            {
                Debug.Log($"Interface: {ni.Name}");
                Debug.Log($"Type: {ni.NetworkInterfaceType}");
                Debug.Log($"Status: {ni.OperationalStatus}");
                
                IPInterfaceProperties ipProps = ni.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Debug.Log($"IPv4 Address: {addr.Address}");
                    }
                }
                Debug.Log("-------------------");
            }
        }
    }

    private void InitializeNatNet()
    {
        try
        {
            Debug.Log($"Attempting to connect to Motive at {serverIP}:{dataPort}");
            
            // Create data socket
            dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            dataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            dataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            dataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            // Create server endpoint
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), dataPort);
            
            // Bind socket to loopback interface
            dataSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), dataPort));
            Debug.Log($"Data socket bound to port {dataPort} on loopback interface");

            // Initialize remote endpoint for receiving data
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Debug.Log($"Ready to receive data from Motive at {serverIP}:{dataPort}");
            Debug.Log($"Data socket endpoint: {dataSocket.LocalEndPoint}");
            Debug.Log($"Server endpoint: {serverEndPoint}");

            isConnected = true;
            isInitialized = true;
            lastDataTime = Time.time;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize NatNet: {e.Message}");
            isInitialized = false;
            if (dataSocket != null)
            {
                try
                {
                    dataSocket.Close();
                }
                catch { }
            }
        }
    }

    private void Update()
    {
        if (!isInitialized || !isConnected) return;

        // Check connection status periodically
        if (Time.time - lastDataTime > connectionCheckInterval)
        {
            Debug.LogWarning($"No data received for {connectionCheckInterval} seconds. Checking connection...");
            Debug.Log($"Data socket state: Available={dataSocket.Available}, LocalEndPoint={dataSocket.LocalEndPoint}");
            Debug.Log($"Server endpoint: {serverEndPoint}");
            CheckConnection();
            lastDataTime = Time.time;
        }

        try
        {
            if (dataSocket.Available > 0)
            {
                Debug.Log($"Data available on data socket: {dataSocket.Available} bytes");
                int bytesRead = dataSocket.ReceiveFrom(receiveBuffer, ref remoteEndPoint);
                if (bytesRead > 0)
                {
                    Debug.Log($"Received {bytesRead} bytes of data from {remoteEndPoint}");
                    lastDataTime = Time.time;
                    ProcessData(receiveBuffer, bytesRead);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving data: {e.Message}");
            CheckConnection();
        }
    }

    private void CheckConnection()
    {
        try
        {
            // For UDP, we don't need to check Connected state
            // Just verify the socket is still valid
            if (dataSocket == null || dataSocket.Handle == IntPtr.Zero)
            {
                Debug.LogWarning("Socket invalid. Attempting to reconnect...");
                InitializeNatNet();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection check failed: {e.Message}");
        }
    }

    private void ProcessData(byte[] data, int length)
    {
        try
        {
            // Skip message ID and packet size
            int offset = 2;

            // Read frame of data
            sFrameOfMocapData frame = new sFrameOfMocapData();
            frame = ByteArrayToStructure<sFrameOfMocapData>(data, offset);
            offset += Marshal.SizeOf(typeof(sFrameOfMocapData));

            Debug.Log($"Processing frame: {frame.iFrame}, RigidBodies: {frame.nRigidBodies}");

            // Process rigid bodies
            for (int i = 0; i < frame.nRigidBodies; i++)
            {
                sRigidBodyData rb = ByteArrayToStructure<sRigidBodyData>(data, offset);
                offset += Marshal.SizeOf(typeof(sRigidBodyData));

                if (rb.ID == rigidBodyID)
                {
                    // Convert position from millimeters to meters
                    Vector3 position = new Vector3(rb.x / 1000f, rb.y / 1000f, rb.z / 1000f);
                    
                    // Convert quaternion (Optitrack uses right-handed coordinate system)
                    Quaternion rotation = new Quaternion(rb.qx, rb.qy, rb.qz, rb.qw);
                    
                    // Apply position and rotation to the GameObject
                    transform.position = position;
                    transform.rotation = rotation;
                    Debug.Log($"Updated rigid body {rb.ID} position: {position}, rotation: {rotation.eulerAngles}");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing data: {e.Message}");
        }
    }

    private T ByteArrayToStructure<T>(byte[] bytes, int offset)
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject() + offset);
        }
        finally
        {
            handle.Free();
        }
    }

    private void OnDestroy()
    {
        if (dataSocket != null)
        {
            try
            {
                dataSocket.Close();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error closing data socket: {e.Message}");
            }
        }
    }
}
