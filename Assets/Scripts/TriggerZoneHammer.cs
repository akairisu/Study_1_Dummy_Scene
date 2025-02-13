using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneHammer : MonoBehaviour
{
    public float TargetHeight = 0.2f;
    public float DisplaceWeight = 0.01f;
    public Transform Nail;
    public void HitNail(Vector3 velocity)
    {
        float directionalVelocity = Vector3.Dot(velocity, -this.transform.up);
        Debug.Log(velocity + " " + -this.transform.up + " " + directionalVelocity);
        float displacement = Mathf.Abs(directionalVelocity) * DisplaceWeight;
        if(displacement > TargetHeight)
        {
            displacement = TargetHeight;
            Destroy(gameObject);
        }
        Vector3 newHeight = Nail.localPosition;
        newHeight.y -= displacement;
        Nail.transform.localPosition = newHeight;
        TargetHeight -= displacement;
    }
}
