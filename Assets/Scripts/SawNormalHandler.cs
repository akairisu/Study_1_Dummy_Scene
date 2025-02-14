using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawNormalHandler : MonoBehaviour
{
    public Transform VirtualObject;
    public Transform DummyObject;
    public List<Renderer> RemoveList;
    public EventHandler EventHandler;

    // Update is called once per frame
    public void SetLocalTransform()
    {
        VirtualObject.localPosition = DummyObject.localPosition;
        VirtualObject.localRotation = DummyObject.localRotation;
        VirtualObject.localScale = DummyObject.localScale;
        EventHandler.RemoveList = RemoveList;
    }
}
