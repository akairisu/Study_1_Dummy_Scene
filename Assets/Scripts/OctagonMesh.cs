using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OctagonMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Octagon Vertices (Assuming 2D shape on XZ plane)
        Vector3[] vertices = new Vector3[]
        {
            new Vector3( 1, 0,  0.5f),
            new Vector3( 0.5f, 0,  1),
            new Vector3(-0.5f, 0,  1),
            new Vector3(-1, 0,  0.5f),
            new Vector3(-1, 0, -0.5f),
            new Vector3(-0.5f, 0, -1),
            new Vector3( 0.5f, 0, -1),
            new Vector3( 1, 0, -0.5f),
            new Vector3( 0, 0,  0), // Center point
        };

        // Triangles (Connecting Vertices)
        int[] triangles = new int[]
        {
            0, 1, 8,
            1, 2, 8,
            2, 3, 8,
            3, 4, 8,
            4, 5, 8,
            5, 6, 8,
            6, 7, 8,
            7, 0, 8
        };

        // Assign to Mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}