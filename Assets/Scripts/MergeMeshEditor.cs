using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Collections;

public class MergeMeshEditor : EditorWindow
{
    private float _windowWidth;
    private GameObject _mergeObject;
    private Texture2D _mergeTexture;

    [MenuItem("Window/MergeMesh")]

    public static void ShowWindow()
    {
        GetWindow<MergeMeshEditor>("Merge Mesh");
    }

    void OnGUI()
    {
        _windowWidth = position.width;
        _mergeObject = (GameObject)EditorGUILayout.ObjectField("Merge Object", _mergeObject, typeof(GameObject), true, GUILayout.Width(_windowWidth - 10));
        _mergeTexture = (Texture2D)EditorGUILayout.ObjectField("Merge Texture", _mergeTexture, typeof(Texture2D), true, GUILayout.Width(_windowWidth - 10));
        if (GUILayout.Button("Generate Merged Texture", GUILayout.Width(_windowWidth - 2)))
        {
            CreateMergedTexture();
        }
        if (GUILayout.Button("Generate Merged Object", GUILayout.Width(_windowWidth - 2)))
        {
            CreateMergedObject();
        }
    }

    private Mesh CreateMergedMesh()
    {
        MeshFilter[] meshFilters = _mergeObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh mergedMesh = new Mesh();
        mergedMesh.CombineMeshes(combineInstances, true, true);

        //_mergeObject.AddComponent<MeshFilter>().sharedMesh = mergedMesh;

        //_mergeObject.AddComponent<MeshRenderer>().sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        //_mergeObject.AddComponent<MeshCollider>().sharedMesh = mergedMesh;

        var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/MergedMesh.mesh");
        AssetDatabase.CreateAsset(mergedMesh, uniqueFileName);

        return mergedMesh;
    }

    private int CountChild(Transform parent)
    {
        if(parent.childCount == 0)
        {
            return 1;
        }

        int count = 0;
        foreach(Transform child in parent)
        {
            count += CountChild(child);
        }

        return count;

    }

    private void CreateMergedTexture()
    {
        int childCount = CountChild(_mergeObject.transform);
        int textureWidth = childCount;
        int textureHeight = childCount;

        Texture2D mergedTexture = new Texture2D(textureWidth, textureHeight);
        Renderer[] renderers = _mergeObject.GetComponentsInChildren<Renderer>();
        Color[] colors = new Color[textureWidth * textureHeight];

        for (int i = 0; i < renderers.Length && i < colors.Length; i++)
        {
            if(renderers[i].sharedMaterial != null)
            {
                colors[i] = renderers[i].sharedMaterial.color;
            }
            else
            {
                colors[i] = Color.white;
            }
        }

        mergedTexture.SetPixels(colors);
        mergedTexture.Apply();

        var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/MergedTexture.png");
        SaveTextureAsPNG(mergedTexture, uniqueFileName);

        return;
    }

    private void SaveTextureAsPNG(Texture2D newTexture, string path)
    {
        byte[] bytes = newTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log("Saved in " + path + "!");
    }

    private void CreateMergedObject()
    {
        GameObject mergedObject = new GameObject();

        MeshFilter meshFilter = mergedObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = CreateMergedMesh();

        MeshRenderer meshRenderer = mergedObject.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = _mergeTexture;
        meshRenderer.material = material;

        var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/MergedMaterial.mat");
        AssetDatabase.CreateAsset(material, uniqueFileName);

        mergedObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;

        uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/MergedObject.prefab");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(mergedObject, uniqueFileName);
        Debug.Log("Saved prefab at " + uniqueFileName);

        DestroyImmediate(mergedObject);
    }
}
