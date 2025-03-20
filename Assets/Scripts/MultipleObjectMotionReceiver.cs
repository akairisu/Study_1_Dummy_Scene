using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class MultiObjectMotionReceiver : MonoBehaviour
{
    // Manually assign the target objects in the Inspector.
    public List<GameObject> objectsToUpdate;

    // Dictionary for fast lookup based on the object id.
    private Dictionary<string, GameObject> objectsByID;

    // Queue for storing pending updates to be processed on the main thread
    private Queue<Action> pendingUpdates = new Queue<Action>();

    UdpClient udpClient;
    IPEndPoint localEndPoint;

    void Start()
    {
        // Initialize the UDP client to listen on localhost (127.0.0.1) port 9050.
        localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
        udpClient = new UdpClient(localEndPoint);
        
        try
        {
            udpClient.BeginReceive(new AsyncCallback(ReceiveData), null);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to start UDP listener: " + e.Message);
        }

        // Build a dictionary for quick lookup using object names (or custom ids if needed).
        objectsByID = new Dictionary<string, GameObject>();
        foreach (GameObject obj in objectsToUpdate)
        {
            if (obj != null)
            {
                // Here, we use the GameObject's name as its identifier.
                objectsByID[obj.name] = obj;
            }
        }
    }

    void Update()
    {
        // Process any pending updates on the main thread
        lock (pendingUpdates)
        {
            while (pendingUpdates.Count > 0)
            {
                Action update = pendingUpdates.Dequeue();
                update?.Invoke();
            }
        }
    }

    void ReceiveData(IAsyncResult result)
    {
        try
        {
            byte[] data = udpClient.EndReceive(result, ref localEndPoint);
            string message = Encoding.UTF8.GetString(data);

            // Deserialize the JSON message into our wrapper object.
            MotionDataWrapper wrapper = JsonUtility.FromJson<MotionDataWrapper>(message);
            if (wrapper != null && wrapper.motions != null)
            {
                foreach (MotionData motion in wrapper.motions)
                {
                    // Queue the transform update to be processed on the main thread
                    if (objectsByID.TryGetValue(motion.id, out GameObject obj))
                    {
                        Vector3 pos = motion.pos;
                        Quaternion rot = motion.rot;
                        string id = motion.id;
                        
                        lock (pendingUpdates)
                        {
                            pendingUpdates.Enqueue(() => {
                                obj.transform.position = pos;
                                obj.transform.rotation = rot;
                            });
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"No assigned object found with id: {motion.id}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Received invalid or empty motion data");
            }

            // Continue listening for incoming data.
            try
            {
                udpClient.BeginReceive(new AsyncCallback(ReceiveData), null);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to continue listening: {e.Message}");
                // Try to restart the listener after a short delay
                Invoke("RestartListener", 1f);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing UDP message: {e.Message}");
            // Try to restart the listener after a short delay
            Invoke("RestartListener", 1f);
        }
    }

    private void RestartListener()
    {
        if (udpClient != null)
        {
            try
            {
                udpClient.BeginReceive(new AsyncCallback(ReceiveData), null);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to restart UDP listener: {e.Message}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}

// Data structure representing the motion of a single object.
[System.Serializable]
public class MotionData
{
    public string id;    // Identifier (e.g., object's name).
    public Vector3 pos;
    public Quaternion rot;
}

// Wrapper class to hold a list of MotionData (needed for JsonUtility).
[System.Serializable]
public class MotionDataWrapper
{
    public List<MotionData> motions;
}
