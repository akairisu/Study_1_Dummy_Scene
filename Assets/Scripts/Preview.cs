using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{
    //Show the selected match & disable the other
    public void IsSelected()
    {
        Transform parent = gameObject.transform.parent;
        Debug.Log(parent);
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
    }
}
