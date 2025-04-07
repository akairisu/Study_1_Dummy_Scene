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

  [Header("3. Fit Anchors to Objects")]
  [Tooltip("Objects to use as reference for anchor positioning")]
  public GameObject leftObject;
  public GameObject rightObject;

  [Tooltip("Additional objects to transform (may include parents of anchors)")]
  public List<GameObject> objectsToTransform = new List<GameObject>();
  public bool fitAnchorsRotation = false;
  public bool fitAnchorsPosition = false;

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

  public void FitAnchorsRotation() {
    if (leftObject == null || rightObject == null || leftAnchor == null || rightAnchor == null) {
      Debug.LogError("Missing required references: leftObject, rightObject, leftAnchor, or rightAnchor");
      return;
    }

    // Calculate the direction vector between the reference objects
    Vector3 referenceDirection = rightObject.transform.position - leftObject.transform.position;
    
    // Calculate the direction vector between the anchors
    Vector3 anchorDirection = rightAnchor.transform.position - leftAnchor.transform.position;
    
    // Calculate the rotation needed to align the anchor direction with the reference direction
    Quaternion rotationDifference = Quaternion.FromToRotation(anchorDirection, referenceDirection);
    
    // Apply the same rotation to all additional objects
    foreach (GameObject obj in objectsToTransform) {
      if (obj != null) {
        obj.transform.rotation = rotationDifference * obj.transform.rotation;
      }
    }
    
    Debug.Log($"Anchors rotation fitted to reference objects and rotation applied to {objectsToTransform.Count} additional objects");
  }

  public void FitAnchorsPosition() {
    if (leftObject == null || leftAnchor == null) {
      Debug.LogError("Missing required references: leftObject or leftAnchor");
      return;
    }

    // Calculate the position difference to move anchors to match reference objects
    Vector3 positionDifference = leftObject.transform.position - leftAnchor.transform.position;

    // Apply the same position adjustment to all additional objects
    foreach (GameObject obj in objectsToTransform) {
      if (obj != null) {
        obj.transform.position += positionDifference;
      }
    }
    
    Debug.Log($"Anchors position fitted to reference objects and position adjustment applied to {objectsToTransform.Count} additional objects");
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
    
    if (fitAnchorsRotation) {
      FitAnchorsRotation();
      fitAnchorsRotation = false;
    }
    
    if (fitAnchorsPosition) {
      FitAnchorsPosition();
      fitAnchorsPosition = false;
    }
  }
}