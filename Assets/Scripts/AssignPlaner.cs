using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlaner : MonoBehaviour
{
    public GameObject[] TriggerZones;
    public EventHandler EventHandler;
    public void Assign()
    {
        foreach (GameObject triggerZone in TriggerZones)
        {
            triggerZone.GetComponent<TriggerZonePlaner>().Planer = EventHandler.InteractionObject.transform;
        }
    }
}
