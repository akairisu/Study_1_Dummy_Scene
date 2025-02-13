using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanerCollisionHandler : MonoBehaviour
{
    public Material TriggerMaterial;
    public Material UntriggerMaterial;
    private void OnTriggerEnter(Collider other)
    {
        Renderer renderer = other.GetComponent<Renderer>();
        renderer.material = TriggerMaterial;
        other.GetComponent<TriggerZonePlaner>().IsTriggered = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Transform targetPlane = other.GetComponent<TriggerZonePlaner>().TargetPlane;
        float signedAngleDifference = Vector3.SignedAngle(this.transform.forward, targetPlane.transform.right, targetPlane.transform.forward);
        Debug.Log("Planer:" + this.transform.up);
        Debug.Log("Plane:" + targetPlane.transform.right);
        Debug.Log("Forward:" + targetPlane.transform.forward);
        Debug.Log("Signed Angle Difference:" + signedAngleDifference);
        if (Mathf.Abs(signedAngleDifference) > 15 && Mathf.Abs(signedAngleDifference) < 165)
        {
            other.GetComponent<TriggerZonePlaner>().IsTriggered = false;
            Renderer renderer = other.GetComponent<Renderer>();
            renderer.material = UntriggerMaterial;
        }
        else
        {
            Renderer renderer = other.GetComponent<Renderer>();
            renderer.material = TriggerMaterial;
            other.GetComponent<TriggerZonePlaner>().IsTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Renderer renderer = other.GetComponent<Renderer>();
        renderer.material = UntriggerMaterial;
        other.GetComponent<TriggerZonePlaner>().IsTriggered = false;
    }
}
