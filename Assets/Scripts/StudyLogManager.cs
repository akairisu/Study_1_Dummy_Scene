using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class StudyLogManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToTrack = new List<GameObject>();
    [SerializeField]
    private SelectRecorder selectRecorder;
    private string logFilePath;

    private void Start()
    {
        // Create a logs directory if it doesn't exist
        string logsDirectory = Path.Combine(Application.dataPath, "..", "Logs");
        if (!Directory.Exists(logsDirectory))
        {
            Directory.CreateDirectory(logsDirectory);
        }

        // Create a log file with timestamp in the filename
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        logFilePath = Path.Combine(logsDirectory, $"object_activity_log_{timestamp}.txt");
    }

    public void LogActiveObjects()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logMessage = $"[{timestamp}] Active Objects:\n";

        foreach (GameObject obj in objectsToTrack)
        {
            if (obj != null && obj.activeInHierarchy)
            {
                logMessage += $"- {obj.name}\n";
            }
        }

        // Append the log message to the file
        File.AppendAllText(logFilePath, logMessage + "\n");
        Debug.Log($"Logged active objects to: {logFilePath}");
    }

    public void setSwordMapping(){
        if(selectRecorder == null){
            Debug.LogError("SelectRecorder is not assigned");
            return;
        }
        foreach (GameObject obj in objectsToTrack)
        {
            selectRecorder.SwordMapping = obj;
        } 
    }

    public void setShieldMapping(){
        if(selectRecorder == null){
            Debug.LogError("SelectRecorder is not assigned");
            return;
        }
        foreach (GameObject obj in objectsToTrack)
        {
            selectRecorder.ShieldMapping = obj;
        }
    }

    public void setMagicCircleMapping(){
        if(selectRecorder == null){
            Debug.LogError("SelectRecorder is not assigned");
            return;
        }
        foreach (GameObject obj in objectsToTrack)
        {
            selectRecorder.MagicCircleMapping = obj;
        }
    }
    // Optional: Add a button in the editor to trigger logging
    [ContextMenu("Log Active Objects")]
    private void LogActiveObjectsFromEditor()
    {
        LogActiveObjects();
    }
}
