using UnityEngine;

public class MenuStateManager : MonoBehaviour
{
    [Header("Target Object to Manage")]
    public GameObject targetObject; // Object to activate/deactivate

    [Header("State Management")]
    public bool confirmed = false; // Initial state

    // Activate target if not confirmed
    public void ActivateTarget()
    {
        if (!confirmed && targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    // Toggle confirmed state (call when grabbing/ungrabbing)
    public void ToggleConfirmed()
    {
        if (targetObject == null) return;

        confirmed = !confirmed;

        // If confirmed, deactivate target; if not, activate it
        targetObject.SetActive(confirmed);
    }
}
