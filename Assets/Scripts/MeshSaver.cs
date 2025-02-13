using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.InputSystem;

public class MeshSaver : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SaveMesh();
        }
    }
    public void SaveMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.mesh == null)
        {
            Debug.LogError("No MeshFilter found!");
            return;
        }

        Mesh mesh = mf.mesh;
        string path = "Assets/" + gameObject.name + ".asset";

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        Debug.Log("Mesh saved to " + path);
    }
}
