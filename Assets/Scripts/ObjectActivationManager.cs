using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NamedObject
{
    public string objectName;
    public GameObject targetObject;
}

public class ObjectActivationManager : MonoBehaviour
{
    [Header("Objects To Manage")]
    public List<NamedObject> managedObjects = new List<NamedObject>();

    [Header("Default Active Object")]
    public string defaultActiveObjectName;

    private Dictionary<string, GameObject> objectLookup = new Dictionary<string, GameObject>();

    void Awake()
    {
        foreach (NamedObject namedObj in managedObjects)
        {
            if (namedObj.targetObject != null && !objectLookup.ContainsKey(namedObj.objectName))
            {
                objectLookup.Add(namedObj.objectName, namedObj.targetObject);
                namedObj.targetObject.SetActive(false);
            }
        }
        
        ActivateObjectByName(defaultActiveObjectName);
    }

    public void ActivateObjectByName(string objectName)
    {
        foreach (var objPair in objectLookup)
            objPair.Value.SetActive(false);

        if (objectLookup.ContainsKey(objectName))
        {
            objectLookup[objectName].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Object name '{objectName}' not found!");
        }
    }
}
