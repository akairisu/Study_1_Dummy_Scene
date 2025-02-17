using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public GameObject InteractionObject;
    public GameObject Menu;
    public List<Renderer> RemoveList = new List<Renderer>();

    public void RemoveRenderer()
    {
        Debug.Log("Removing!" + RemoveList.Count);
        foreach (Renderer renderer in RemoveList)
        {
            Debug.Log("Removed " + renderer.name);
            renderer.enabled = false;
        }
        Destroy(Menu);
    }

    public void RecoverRenderer()
    {
        foreach (Renderer renderer in RemoveList)
        {
            renderer.enabled = true;
        }
    }
}
