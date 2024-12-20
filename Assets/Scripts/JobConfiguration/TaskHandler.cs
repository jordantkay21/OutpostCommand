using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TaskHandler : MonoBehaviour
{
    private Queue<TaskBase> taskQueue = new Queue<TaskBase>();
    private bool isProcessingTask = false;

    public void AddTask(TaskBase task)
    {
        taskQueue.Enqueue(task);
        ProcessNextTask();
    }

    public void AddTasks(IEnumerable<TaskBase> tasks)
    {
        foreach (var task in tasks)
        {
            taskQueue.Enqueue(task);
        }
        ProcessNextTask();
    }

    private void ProcessNextTask()
    {
        if (isProcessingTask || taskQueue.Count == 0)
            return;

        isProcessingTask = true;
        TaskBase currentTask = taskQueue.Dequeue();
        currentTask.Execute(GetComponent<Survivor>(), () =>
        {
            isProcessingTask = false;
            ProcessNextTask(); // Start next task
        });
    }
}
