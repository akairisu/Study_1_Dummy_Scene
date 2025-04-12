using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulation : MonoBehaviour
{
    public GameObject handle;
    public GameObject body;
    public GameObject plane;

    public float planeYDelta = 0.0f;
    public float handleZDelta = 0.0f;
    public float bodyZDelta = 0.0f;

    public bool start = false;
    
    private Vector3 handleInitialPos;
    private Vector3 bodyInitialPos;
    private Vector3 planeInitialPos;
    
    private float lerpDuration = 1.0f;
    private float elapsedTime = 0.0f;
    private bool isPlaneMoving = false;
    private bool isHandleBodyMoving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Store initial positions
        handleInitialPos = handle.transform.position;
        bodyInitialPos = body.transform.position;
        planeInitialPos = plane.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (start) {
            DoManipulation();
            start = false;
        }
        
        if (isPlaneMoving) {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / lerpDuration;
            
            // Move only the plane
            Vector3 planeTarget = planeInitialPos + new Vector3(0, planeYDelta, 0);
            plane.transform.position = Vector3.Lerp(planeInitialPos, planeTarget, percentageComplete);
            
            // Check if plane movement is complete
            if (percentageComplete >= 1.0f) {
                isPlaneMoving = false;
                elapsedTime = 0.0f;
                // Start handle and body movement
                isHandleBodyMoving = true;
            }
        }
        else if (isHandleBodyMoving) {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / lerpDuration;
            
            // Move handle and body
            Vector3 handleTarget = handleInitialPos + new Vector3(0, 0, handleZDelta);
            Vector3 bodyTarget = bodyInitialPos + new Vector3(0, 0, bodyZDelta);
            
            handle.transform.position = Vector3.Lerp(handleInitialPos, handleTarget, percentageComplete);
            body.transform.position = Vector3.Lerp(bodyInitialPos, bodyTarget, percentageComplete);
            
            // Check if handle and body movement is complete
            if (percentageComplete >= 1.0f) {
                isHandleBodyMoving = false;
                elapsedTime = 0.0f;
            }
        }
    }

    void DoManipulation() {
        // Reset elapsed time and start with plane movement
        elapsedTime = 0.0f;
        isPlaneMoving = true;
        isHandleBodyMoving = false;
    }
}
