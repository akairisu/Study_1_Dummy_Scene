using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawNoMatchHandler : MonoBehaviour
{
    public Transform NormalSaw;
    public Transform NoMatchSaw;
    public EventHandler EventHandler;
    //Show the selected match & disable the other
    public void IsSelected()
    {
        Transform parent = gameObject.transform.parent;
        foreach (Transform child in parent)
        {
            // Check if the current child is the selected child
            if (child.gameObject == gameObject)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
        EventHandler.GameSaw = NoMatchSaw;
        EventHandler.RemoveList = new List<Renderer>();
    }

    public void DeSelect()
    {
        Transform parent = gameObject.transform.parent;
        foreach (Transform child in parent)
        {
            // Check if the current child is the selected child
            if (child.gameObject == gameObject)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
        EventHandler.GameSaw = NormalSaw;
    }
}
