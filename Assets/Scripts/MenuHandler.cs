using UnityEngine;
using Oculus.Interaction.Input;

public class MenuHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuObject;  // The menu to show/hide
    [SerializeField] private GameObject targetObject;  // The object that triggers the menu
    [SerializeField] private HandRef rightHand;     // Reference to the OVR right hand
    [SerializeField] private HandRef leftHand;      // Reference to the OVR left hand
    
    [Header("Settings")]
    [SerializeField] private float proximityThreshold = 0.1f; // Distance threshold in meters
    
    private bool isSystemActive = false;  // Control whether the proximity detection is active
    private bool isMenuVisible = false;   // Track menu visibility state

    private void Start()
    {
        // Ensure menu starts hidden
        if (menuObject != null)
        {
            menuObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isSystemActive) return;

        // Check if either hand is close enough
        bool shouldShowMenu = CheckHandProximity(rightHand) || CheckHandProximity(leftHand);

        // Update menu visibility if needed
        if (shouldShowMenu != isMenuVisible)
        {
            isMenuVisible = shouldShowMenu;
            menuObject.SetActive(isMenuVisible);
        }
    }

    private bool CheckHandProximity(HandRef hand)
    {
        if (hand == null || targetObject == null) return false;

        // Check if hand is being tracked
        Pose pose;
        if (!hand.GetRootPose(out pose)) return false;

        // Get hand position from the pose
        Vector3 handPosition = pose.position;
        
        // Calculate distance between hand and target slot
        float distance = Vector3.Distance(handPosition, targetObject.transform.position);
        
        // Return true if hand is within threshold distance
        return distance <= proximityThreshold;
    }

    // Public function to enable/disable the proximity detection system
    public void SetSystemActive(bool active)
    {
        isSystemActive = active;
        
        // If system is deactivated, ensure menu is hidden
        if (!active && menuObject != null)
        {
            isMenuVisible = false;
            menuObject.SetActive(false);
        }
    }

    // Public function to get current system state
    public bool IsSystemActive()
    {
        return isSystemActive;
    }
}
