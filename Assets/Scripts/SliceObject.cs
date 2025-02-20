using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

public class SliceObject : MonoBehaviour
{
    public Material CrossSectionMaterial;
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public GameObject Slicer;
    public LayerMask sliceableLayer;
    public Transform DebugPlane;
    public GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if (hasHit)
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Slice_Debug();
        }
    }

    public void Slice(GameObject target)
    {
        Vector3 sawNorm = Slicer.transform.right;
        SlicedHull hull = target.Slice(endSlicePoint.position, sawNorm);
        if(hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, CrossSectionMaterial);
            GameObject lowerHull = hull.CreateLowerHull(target, CrossSectionMaterial);
        }
    }

    public void Slice_Debug()
    {
        SlicedHull hull = Target.Slice(DebugPlane.position, DebugPlane.up);
        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(Target, CrossSectionMaterial);
            GameObject lowerHull = hull.CreateLowerHull(Target, CrossSectionMaterial);
        }
    }
}