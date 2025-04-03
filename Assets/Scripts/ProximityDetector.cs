using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ProximityDetector : MonoBehaviour
{
    [Tooltip("List of GameObjects to check proximity against")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Tooltip("Distance threshold for proximity detection (in units)")]
    [SerializeField] private float proximityThreshold = 5f;

    [Tooltip("Whether to draw debug visualization in Scene view")]
    [SerializeField] private bool showDebugVisuals = true;

    public bool isActive = true;

    [Tooltip("Event triggered when an object enters proximity range")]
    public UnityEvent<GameObject> onObjectEnterRange;

    [Tooltip("Event triggered when an object exits proximity range")]
    public UnityEvent<GameObject> onObjectExitRange;

    // Keep track of which objects are currently in range
    private HashSet<GameObject> objectsInRange = new HashSet<GameObject>();

    private void Update()
    {
        if (!isActive) return;

        bool anyObjectInRange = false;
        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                bool wasInRange = objectsInRange.Contains(target);
                
                if (distance <= proximityThreshold)
                {
                    // Object is within proximity threshold
                    anyObjectInRange = true;
                    if (!wasInRange)
                    {
                        // Object just entered range
                        objectsInRange.Add(target);
                        onObjectEnterRange?.Invoke(target);
                    }
                }
                else if (wasInRange)
                {
                    // Object just exited range
                    objectsInRange.Remove(target);
                }
            }
        }

        // Only trigger exit event if no objects are in range
        if (!anyObjectInRange && objectsInRange.Count > 0)
        {
            objectsInRange.Clear();
            onObjectExitRange?.Invoke(null);
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugVisuals)
        {
            // Draw a wire sphere to visualize the proximity threshold
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, proximityThreshold);
        }
    }

    public void DisableProximityDetection()
    {
        isActive = false;
    }

    public void EnableProximityDetection()
    {
        isActive = true;
    }
} 