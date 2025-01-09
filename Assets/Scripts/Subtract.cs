using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Subtract : MonoBehaviour
{
    public GameObject BoundingBox;
    private Bounds _bounds;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("Mesh filter not found.");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector2[] uv2 = mesh.uv2;
        Vector2[] uv3 = mesh.uv3;
        bool uv2Flag = false;
        bool uv3Flag = false;
        int[] triangles = mesh.triangles;
        _bounds = new Bounds(BoundingBox.transform.position, BoundingBox.transform.localScale);

        if(uv2.Length == 0)
        {
            Debug.Log("No uv2!");
        }
        else
        {
            uv2Flag = true;
        }
        if(uv3.Length == 0)
        {
            Debug.Log("No uv3!");
        }
        else
        {
            uv3Flag = true;
        }

        // Iterate through vertices and check if each vertex is inside the bounding box
        for (int i = 0; i < vertices.Length; i++)
        {
            if (!_bounds.Contains(transform.TransformPoint(vertices[i])))
            {
                // Remove the vertex by setting it to Vector3.zero
                vertices[i] = Vector3.zero;
            }
        }

        // Remove degenerate triangles
        bool[] removeTriangles = new bool[triangles.Length / 3];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (vertices[triangles[i]] == Vector3.zero ||
                vertices[triangles[i + 1]] == Vector3.zero ||
                vertices[triangles[i + 2]] == Vector3.zero)
            {
                triangles[i] = triangles[i + 1] = triangles[i + 2] = -1;
                removeTriangles[i / 3] = true;
            }
        }

        // Count the number of triangles to keep
        int numTriangles = 0;
        for (int i = 0; i < removeTriangles.Length; i++)
        {
            if (!removeTriangles[i])
            {
                numTriangles += 3;
            }
        }

        /*
        //Create hash table for new vertices
        int[] verticeHash = new int[vertices.Length];
        Vector3[] tempVertices = new Vector3[vertices.Length];
        int hashIndex = 0;
        for(int i = 0; i < vertices.Length; i++)
        {
            verticeHash[i] = -1;
            if(vertices[i] != Vector3.zero)
            {
                verticeHash[i] = hashIndex;
                tempVertices[hashIndex] = vertices[i];
                hashIndex++;
            }
        }
        Vector3[] newVertices = new Vector3[hashIndex];
        Array.Copy(tempVertices, newVertices, hashIndex);

        Debug.Log(newVertices.Length);

        int[] newTriangles = new int[numTriangles];
        int newIndex = 0;
        for(int i = 0; i < triangles.Length; i += 3)
        {
            if(!removeTriangles[i / 3])
            {
                newTriangles[newIndex] = verticeHash[triangles[i]];
                newTriangles[newIndex + 1] = verticeHash[triangles[i + 1]];
                newTriangles[newIndex + 2] = verticeHash[triangles[i + 2]];
                newIndex += 3;
            }
        }

        Debug.Log(newTriangles.Length);
        Debug.Log(newTriangles.Max());*/

        // Create new arrays for vertices and triangles
        Vector3[] newVertices = new Vector3[numTriangles];
        Vector2[] newUV = new Vector2[numTriangles];
        Vector2[] newUV2 = new Vector2[numTriangles];
        Vector2[] newUV3 = new Vector2[numTriangles];
        int[] newTriangles = new int[numTriangles];
        int newIndex = 0;

        // Copy vertices and triangles to new arrays, excluding removed triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (!removeTriangles[i / 3])
            {
                newVertices[newIndex] = vertices[triangles[i]];           
                newVertices[newIndex + 1] = vertices[triangles[i + 1]];
                newVertices[newIndex + 2] = vertices[triangles[i + 2]];

                newUV[newIndex] = uv[triangles[i]];
                newUV[newIndex + 1] = uv[triangles[i + 1]];
                newUV[newIndex + 2] = uv[triangles[i + 2]];

                if (uv2Flag)
                {
                    newUV2[newIndex] = uv2[triangles[i]];
                    newUV2[newIndex + 1] = uv2[triangles[i + 1]];
                    newUV2[newIndex + 2] = uv2[triangles[i + 2]];
                }

                if (uv3Flag)
                {
                    newUV3[newIndex] = uv3[triangles[i]];
                    newUV3[newIndex + 1] = uv3[triangles[i + 1]];
                    newUV3[newIndex + 2] = uv3[triangles[i + 2]];
                }

                newTriangles[newIndex] = newIndex;
                newTriangles[newIndex + 1] = newIndex + 1;
                newTriangles[newIndex + 2] = newIndex + 2;

                newIndex += 3;
            }
        }

        // Update the mesh with the new vertices and triangles
        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        if (uv2Flag)
        {
            mesh.uv2 = newUV2;
        }
        if (uv3Flag)
        {
            mesh.uv3 = newUV3;
        }

        // Rebuild the mesh without the removed vertices
        //mesh.vertices = vertices;
        //mesh.triangles = triangles;

        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
