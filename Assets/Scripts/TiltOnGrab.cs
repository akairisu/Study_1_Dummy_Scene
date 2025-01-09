using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltOnGrab : MonoBehaviour
{
    public Vector3 TiltOffset; //Tilting angle
    private bool touchedFlag = false; // Flag to tilt only for the first time
    public void Tilt()
    {
        if (!touchedFlag)
        {
            Quaternion tiltRotation = Quaternion.Euler(TiltOffset);
            this.transform.localRotation = this.transform.localRotation * tiltRotation;
            touchedFlag = true;
        }
        else
        {
            return;
        }
    }
}
