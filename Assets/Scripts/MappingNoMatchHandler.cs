using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappingNoMatchHandler : MonoBehaviour
{
    public GameObject NormalObject;
    public GameObject NoMatchObject;
    public EventHandler EventHandler;
    //Show the selected match & disable the other
    public void IsSelected()
    {
        gameObject.SetActive(true);
        NormalObject.SetActive(false);
        EventHandler.InteractionObject = NoMatchObject;
        EventHandler.RemoveList = new List<Renderer>();
    }

    public void DeSelect()
    {
        gameObject.SetActive(false);
        NormalObject.SetActive(true);
    }
}
