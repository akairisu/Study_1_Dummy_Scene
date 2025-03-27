using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class TaskEvent
{
    public string name;
    public List<UnityEvent> events;
}
