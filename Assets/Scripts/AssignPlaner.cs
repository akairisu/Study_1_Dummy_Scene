using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlaner : MonoBehaviour
{
    public GameObject[] TriggerZones;
    public EventHandler EventHandler;
    public void Assign()
    {
        // Set the tag on the interaction object
        EventHandler.InteractionObject.tag = "Planer";
        
        // No need to modify trigger zones as they will find the object by tag
    }
}
