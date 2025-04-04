using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class StudyTool : MonoBehaviour
{
  [Header("Anchors")]
  public GameObject rightHand;
  public GameObject leftHand;
  public GameObject rightAnchor;
  public GameObject leftAnchor;
  public GameObject cameraRig;

  [Header("1. Set Rotation in User Study")]
  [Tooltip("This will modify the OVR's transform rotation to match the anchor points")]
  public bool setRotation = false;

  [Header("2. Set Anchor Position in User Study")]
  [Tooltip("This will modify the OVR's transform position to match the anchor point")]
  public bool setAnchor = false;

  public void SetRotation() {
    if (rightHand == null || leftHand == null || rightAnchor == null || leftAnchor == null) {
      Debug.LogError("Missing required references: rightHand, leftHand, rightAnchor, or leftAnchor");
      return;
    }

    // Calculate the direction vector between the anchor points
    Vector3 anchorDirection = rightAnchor.transform.position - leftAnchor.transform.position;
    
    // Calculate the direction vector between the hands
    Vector3 handDirection = rightHand.transform.position - leftHand.transform.position;
    
    // Calculate the rotation needed to align the hand direction with the anchor direction
    Quaternion rotationDifference = Quaternion.FromToRotation(handDirection, anchorDirection);
    
    // Apply the rotation to the camera rig (which contains both hands)
    cameraRig.transform.rotation = rotationDifference * cameraRig.transform.rotation;
    
    Debug.Log($"Rotation set: Aligned hands with anchor points");
  }

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
    if (setRotation) {
      SetRotation();
      setRotation = false;
    }

    if (setAnchor) {
      SetAnchor();
      setAnchor = false;
    }
  }
}