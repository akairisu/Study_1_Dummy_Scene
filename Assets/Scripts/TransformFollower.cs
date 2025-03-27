using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The target object to follow")]
    public Transform target;

    [Header("Offset Settings")]
    [Tooltip("Offset from the target's position")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("Offset from the target's rotation")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("Offset from the target's scale")]
    public Vector3 scaleOffset = Vector3.one;

    private void LateUpdate()
    {
        if (target == null) return;

        // Directly match position
        transform.position = target.position + positionOffset;

        // Directly match rotation
        transform.rotation = target.rotation * Quaternion.Euler(rotationOffset);

        // Directly match scale
        transform.localScale = Vector3.Scale(target.localScale, scaleOffset);
    }
}
