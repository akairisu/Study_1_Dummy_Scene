using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotation : MonoBehaviour
{
    public GameObject CameraRig;

    // Update is called once per frame
    void Update()
    {
        if (CameraRig != null)
        {
            Vector3 direction = transform.position - CameraRig.transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
