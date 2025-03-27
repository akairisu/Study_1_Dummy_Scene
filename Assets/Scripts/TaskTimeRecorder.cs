using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class TaskTimeRecorder : MonoBehaviour
{
    [System.Serializable]
    public class TaskRecord
    {
        public string taskName;
        public float startTime;
        public float endTime;
        public float duration;
    }

    [System.Serializable]
    public class TaskRecordList
    {
        public List<TaskRecord> records = new List<TaskRecord>();
    }

    private Dictionary<string, float> taskStartTimes = new Dictionary<string, float>();
    private TaskRecordList taskRecords = new TaskRecordList();

    public string savePath;

    private void Awake()
    {
        #if UNITY_EDITOR
            savePath = Path.Combine(Application.dataPath, "../TaskTimes.json");
        #else
            savePath = Path.Combine(Application.persistentDataPath, "TaskTimes.json");
        #endif
savePath = Path.Combine(Application.persistentDataPath, "TaskTimes.json");
        Debug.Log($"[TaskTimeRecorder] JSON will be saved to: {savePath}");
    }

    public void StartTask(string taskName)
    {
        taskStartTimes[taskName] = Time.time;
        Debug.Log($"[TaskTimeRecorder] Started task: {taskName} at {Time.time}");
    }

    public void EndTask(string taskName)
    {
        if (taskStartTimes.TryGetValue(taskName, out float startTime))
        {
            float endTime = Time.time;
            float duration = endTime - startTime;

            TaskRecord record = new TaskRecord
            {
                taskName = taskName,
                startTime = startTime,
                endTime = endTime,
                duration = duration
            };

            taskRecords.records.Add(record);
            SaveToFile();
            Debug.Log($"[TaskTimeRecorder] Ended task: {taskName}. Duration: {duration:F2} seconds.");
        }
        else
        {
            Debug.LogWarning($"[TaskTimeRecorder] Tried to end unknown task: {taskName}");
        }
    }

    private void SaveToFile()
    {
        string json = JsonUtility.ToJson(taskRecords, true);
        File.WriteAllText(savePath, json);
    }
}
