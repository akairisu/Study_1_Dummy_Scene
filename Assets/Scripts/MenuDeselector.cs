using UnityEngine;
using System.Collections.Generic;

public class MenuDeselector : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> objectsToDeactivate;

    public void DeactivateObjects()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
