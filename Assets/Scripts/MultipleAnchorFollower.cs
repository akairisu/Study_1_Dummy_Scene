using UnityEngine;
using System.Collections.Generic;

public class MultipleAnchorFollower : MonoBehaviour
{
    [Header("Targets to Follow")]
    [Tooltip("Targets to follow, it will follow the active one. if multiple targets are active, the follower will follow the first target")]
    [SerializeField]
    private List<Transform> targets;

    [Header("Position Offset")]
    public Vector3 positionOffset; // Offset from the target's position

    [Header("Rotation Tracking")]
    public bool trackRotationChanges = true; // Track only rotation changes, not absolute rotation
    private Quaternion initialTargetRotation;
    private Quaternion initialObjectRotation;
    private Transform currentTarget;

    [Header("Disable the Component")]
    public bool disable = false;

    void Start()
    {
        // Find the first active target
        UpdateCurrentTarget();
        
        if (currentTarget != null)
        {
            // Store the initial rotations
            initialTargetRotation = currentTarget.rotation;
            initialObjectRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (disable)
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        UpdateCurrentTarget();

        if (currentTarget == null) {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
            return;
        };

        // Update position with offset
        transform.position = currentTarget.position + currentTarget.TransformDirection(positionOffset);

        // Handle rotation tracking
        if (trackRotationChanges)
        {
            Quaternion deltaRotation = currentTarget.rotation * Quaternion.Inverse(initialTargetRotation);
            transform.rotation = deltaRotation * initialObjectRotation;
        }
    }

    private void UpdateCurrentTarget()
    {
        if (targets == null || targets.Count == 0)
        {
            currentTarget = null;
            return;
        }

        // Find the first active target
        currentTarget = null;
        foreach (Transform target in targets)
        {
            if (target != null && target.gameObject.activeInHierarchy)
            {
                currentTarget = target;
                break;
            }
        }
    }

    public void Disable()
    {
        disable = true;
    }
}
