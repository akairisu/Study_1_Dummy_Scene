using UnityEngine;

public class AnchorFollower : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target; // The object to follow

    [Header("Position Offset")]
    public Vector3 positionOffset; // Offset from the target's position

    [Header("Rotation Tracking")]
    public bool trackRotationChanges = true; // Track only rotation changes, not absolute rotation
    private Quaternion initialTargetRotation;
    private Quaternion initialObjectRotation;

    void Start()
    {
        if (target != null)
        {
            // Store the initial rotations
            initialTargetRotation = target.rotation;
            initialObjectRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (target == null) return;

        // Update position with offset
        transform.position = target.position + target.TransformDirection(positionOffset);

        // Handle rotation tracking
        if (trackRotationChanges)
        {
            Quaternion deltaRotation = target.rotation * Quaternion.Inverse(initialTargetRotation);
            transform.rotation = deltaRotation * initialObjectRotation;
        }
    }
}
