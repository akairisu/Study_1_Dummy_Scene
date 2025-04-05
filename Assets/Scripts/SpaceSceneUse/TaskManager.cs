using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public List<TaskEvent> taskEvents;

    public static TaskManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerTaskByName(string taskName)
    {
        foreach (var task in taskEvents)
        {
            if (task.name == taskName)
            {
                Debug.Log($"Triggering task: {taskName}");

                foreach (var ev in task.events)
                {
                    ev?.Invoke();
                }

                return;
            }
        }

        Debug.LogWarning($"Task with name '{taskName}' not found.");
    }
}
