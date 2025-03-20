using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MagicalTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;  // Drag and drop the specific object you want to detect in the Unity Inspector

    [SerializeField]
    private UnityEvent onTriggerActivated;  // This will show up in the Unity Inspector as an event that you can connect to other objects' functions
    
    [SerializeField]
    private UnityEvent onTriggerDeactivated;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is our target object
        if (other.gameObject == targetObject)
        {
            Debug.Log($"Target object {targetObject.name} has entered the trigger area!");
            // Invoke the UnityEvent when the target object enters
            onTriggerActivated?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            Debug.Log($"Target object {targetObject.name} has left the trigger area!");
            // Invoke the UnityEvent when the target object exits
            onTriggerDeactivated?.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // This function is called every frame while another collider is within the trigger
        // Left empty for performance, uncomment if needed
        // if (other.gameObject == targetObject)
        // {
        //     Debug.Log($"Target object {targetObject.name} is still in the trigger area!");
        // }
    }
}
