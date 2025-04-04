using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class StudyTool : MonoBehaviour
{
  [Header("Anchors")]
  public GameObject rightHand;
  public GameObject rightAnchor;
  public GameObject cameraRig;

  [Header("Set Anchor in User Study")]
  [Tooltip("This will modify the OVR's transform position to match the anchor point")]
  public bool setAnchor = false;

  public void SetAnchor() {
    if (rightHand == null || rightAnchor == null || cameraRig == null) {
      Debug.LogError("Missing required references: rightHand, rightAnchor, or cameraRig");
      return;
    }

    // Calculate the current position difference between the right hand and the anchor
    Vector3 positionDifference = rightAnchor.transform.position - rightHand.transform.position;
    
    // Apply this difference to the camera rig's position
    cameraRig.transform.position += positionDifference;
    
    Debug.Log($"Anchor set: Camera rig moved by {positionDifference} to align right hand with anchor");
  }

  private void Update() {
    if (setAnchor) {
      SetAnchor();
      setAnchor = false;
    }
  }
}