using UnityEngine;

public class UpdatePlaneScale : MonoBehaviour
{
    public Transform TopRight;
    public Transform BottomLeft;
    private GameObject _tablePlane;

    private void Start()
    {
        CreatePlane();
    }

    void CreatePlane()
    {
        if (TopRight == null || BottomLeft == null)
        {
            Debug.LogError("Assign both corners in the Inspector!");
            return;
        }

        _tablePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        // Compute the plane center (midpoint of the two corners)
        Vector3 center = (TopRight.position + BottomLeft.position) / 2f;

        // Compute plane scale (Unity Plane is 10x10 by default, so divide by 10)
        float width = Mathf.Abs(TopRight.position.x - BottomLeft.position.x) / 10f;
        float height = Mathf.Abs(TopRight.position.z - BottomLeft.position.z) / 10f;

        Debug.Log("Plane created at: " + center + " with size: " + width + " x " + height);
    }

    void Update()
    {
        if (TopRight == null || BottomLeft == null)
        {
            Debug.LogError("Assign both corner objects in the Inspector!");
            return;
        }

        // Compute the center position
        Vector3 center = (TopRight.position + BottomLeft.position) / 2f;

        // Compute new scale (Unity's default Plane is 10x10, so divide by 10)
        float width = Mathf.Abs(TopRight.position.x - BottomLeft.position.x) / 10f;
        float height = Mathf.Abs(TopRight.position.z - BottomLeft.position.z) / 10f;

        // Apply changes to the existing plane
        _tablePlane.transform.position = center;
        _tablePlane.transform.localScale = new Vector3(width, 1, height);

        Debug.Log("Plane updated: Position = " + center + ", Scale = " + _tablePlane.transform.localScale);
    }
}
