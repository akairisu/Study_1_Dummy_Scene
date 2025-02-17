using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappingNormalHandler : MonoBehaviour
{
    public Transform VirtualObject;
    public Transform DummyObject;
    public GameObject Menu;
    public List<Renderer> RemoveList;
    public EventHandler EventHandler;

    public void SetLocalTransform()
    {
        VirtualObject.localPosition = DummyObject.localPosition;
        VirtualObject.localRotation = DummyObject.localRotation;
        VirtualObject.localScale = DummyObject.localScale;
        EventHandler.RemoveList = RemoveList;
        EventHandler.Menu = Menu;
        EventHandler.InteractionObject = VirtualObject.gameObject;
    }
}
