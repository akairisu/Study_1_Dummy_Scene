using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class StudyTool : MonoBehaviour
{
  [Header("Scene Anchors")]
  public GameObject rightAnchor;
  public GameObject leftAnchor;
  public GameObject trackedRightAnchor;
  public GameObject trackedLeftAnchor;
  public GameObject scene;

  [Header("OVR Anchors")]
  public GameObject rightHand;
  public GameObject leftHand;
  public GameObject cameraRig;

  [Header("1. Calibrate Scene")]
  public bool calibrateTable = false;

  [Header("2. Calibrate OVR")]
  public bool calibrateOVR = false;

  [Header("3. Additional Control")]
  public float levelingDelta = 0.01f;
  public bool levelUpOVR = false;
  public bool levelDownOVR = false;
  public bool showTrackedAnchors = false;
  public bool hideTrackedAnchors = false;

  public void SetSceneRotation() {
    if (trackedRightAnchor == null || trackedLeftAnchor == null || rightAnchor == null || leftAnchor == null || scene == null) {
      Debug.LogError("Missing required references: trackedRightAnchor, trackedLeftAnchor, rightAnchor, leftAnchor, or scene");
      return;
    }

    // Calculate the direction vector between the anchor points
    Vector3 anchorDirection = rightAnchor.transform.position - leftAnchor.transform.position;
    
    // Calculate the direction vector between the tracked anchors
    Vector3 trackedDirection = trackedRightAnchor.transform.position - trackedLeftAnchor.transform.position;
    
    // Calculate the rotation needed to align the tracked direction with the anchor direction
    Quaternion rotationDifference = Quaternion.FromToRotation(anchorDirection, trackedDirection);
    
    // Apply the rotation to the scene
    scene.transform.rotation = rotationDifference * scene.transform.rotation;
    
    Debug.Log($"Scene rotation set: Aligned tracked anchors with anchor points");
  }

  public void SetScenePosition() {
    if (trackedRightAnchor == null || rightAnchor == null || scene == null) {
      Debug.LogError("Missing required references: trackedRightAnchor, rightAnchor, or scene");
      return;
    }

    // Calculate the current position difference between the right anchor and the tracked right anchor
    Vector3 positionDifference = rightAnchor.transform.position - trackedRightAnchor.transform.position;
    
    // Apply this difference to the scene's position (moving the scene in the opposite direction)
    scene.transform.position -= positionDifference;
    
    Debug.Log($"Scene position set: Scene moved by {positionDifference} to align tracked right anchor with anchor");
  }

  public void SetOVRRotation() {
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

  public void SetOVRPosition() {
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

  public void LevelUpOVR() {
    if (cameraRig == null) {
      Debug.LogError("Missing required references: cameraRig");
      return;
    }

    cameraRig.transform.position += new Vector3(0, levelingDelta, 0);
  }
  
  public void LevelDownOVR() {
    if (cameraRig == null) {
      Debug.LogError("Missing required references: cameraRig");
      return;
    }
    
    cameraRig.transform.position -= new Vector3(0, levelingDelta, 0);
  }

  private void Update() {
    if (calibrateTable) {
      SetSceneRotation();
      SetScenePosition();
      calibrateTable = false;
    }

    if (calibrateOVR) {
      SetOVRRotation();
      SetOVRPosition();
      calibrateOVR = false;
    }

    if (levelUpOVR) {
      LevelUpOVR();
      levelUpOVR = false;
    }

    if (levelDownOVR) {
      LevelDownOVR();
      levelDownOVR = false;
    }

    if (hideTrackedAnchors) {
      HideTrackedAnchors();
      hideTrackedAnchors = false;
    }

    if (showTrackedAnchors) {
      ShowTrackedAnchors();
      showTrackedAnchors = false;
    }
  }

  private void HideTrackedAnchors() {
    if (trackedRightAnchor == null || trackedLeftAnchor == null) {
      Debug.LogError("Missing required references: trackedRightAnchor or trackedLeftAnchor");
      return;
    }

    trackedRightAnchor.SetActive(false);
    trackedLeftAnchor.SetActive(false);
  }

  private void ShowTrackedAnchors() {
    if (trackedRightAnchor == null || trackedLeftAnchor == null) {
      Debug.LogError("Missing required references: trackedRightAnchor or trackedLeftAnchor");
      return;
    }

    trackedRightAnchor.SetActive(true);
    trackedLeftAnchor.SetActive(true);
  }
}