using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerCollisionHandler : MonoBehaviour
{
    public Material TriggerMaterial;
    public Material UntriggerMaterial;
    public VelocityEstimator velocityEstimator;
    private AudioSource _audioSource; 

    void Start()
    {
           _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Debug.Log("velocity: " + velocity);
        TriggerZoneHammer triggerZoneHammer = other.GetComponent<TriggerZoneHammer>();
        triggerZoneHammer.HitNail(velocity);
        Renderer objectRenderer = other.GetComponent<Renderer>();
        objectRenderer.material = TriggerMaterial;
        _audioSource.Play();
    }
    private void OnTriggerExit(Collider other)
    {
        Renderer objectRenderer = other.GetComponent<Renderer>();
        objectRenderer.material = UntriggerMaterial;
    }
}
