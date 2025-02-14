using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public GameObject[] SawTriggerZone;
    public Transform GameSaw;
    public GameObject SawMenu;
    public List<Renderer> RemoveList = new List<Renderer>();
    // Start is called before the first frame update
    void Start()
    {
        int physicLayer = LayerMask.NameToLayer("Object");
        int virtualLayer = LayerMask.NameToLayer("VirtualObject");

        Physics.IgnoreLayerCollision(physicLayer, virtualLayer, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignGameSaw()
    {
        foreach (GameObject triggerZone in SawTriggerZone)
        {
            triggerZone.GetComponent<TriggerZoneSaw>().Saw = GameSaw;
        }
        RemoveRenderer();
        Destroy(SawMenu);
    }

    private void RemoveRenderer()
    {
        Debug.Log("Removing!" + RemoveList.Count);
        foreach (Renderer renderer in RemoveList)
        {
            Debug.Log("Removed " + renderer.name);
            renderer.enabled = false;
        }
    }

    public void RecoverRenderer()
    {
        foreach (Renderer renderer in RemoveList)
        {
            renderer.enabled = true;
        }
    }
}
