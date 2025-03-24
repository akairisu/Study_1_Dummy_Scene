using UnityEngine;

public class RotateForDemo : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 30f; // Speed in degrees per second
    private bool isRotating = true; // Control rotation toggle

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    // Function to toggle rotation (useful for recording)
    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    // Function to set rotation speed dynamically
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}
