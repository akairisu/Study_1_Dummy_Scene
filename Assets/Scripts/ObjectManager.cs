using UnityEngine;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private List<string> trackedTags = new List<string>();
    [Header("Debug")]
    [SerializeField]
    private string exceptionTag;
    public bool debugDisable = false;

    public void DisableObjectsByTag(string tagName)
    {
        GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag(tagName);
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    public void DisableTrackedTagsExcept(string tagName)
    {
        foreach (string trackedTag in trackedTags)
        {
            if (trackedTag != tagName)
            {
                GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag(trackedTag);
                foreach (GameObject obj in objectsToDisable)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    private void Update() {
        if (debugDisable) {
            DisableTrackedTagsExcept(exceptionTag);
            debugDisable = false;
        }
    }
}
