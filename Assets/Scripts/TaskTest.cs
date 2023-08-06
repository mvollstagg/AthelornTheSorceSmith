using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FireAndForgetTaskExample();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DelayedTaskExample();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RecurringTaskExample();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ContinuationTaskExample();
        }
    }

    public void FireAndForgetTaskExample()
    {
        TaskManager.Instance.ScheduleFireAndForgetTask("FireAndForgetTask", () =>
        {
            Debug.Log("This is a Fire-and-Forget task!");
        });
    }

    public void DelayedTaskExample()
    {
        TaskManager.Instance.ScheduleDelayedTask("DelayedTask", () =>
        {
            Debug.Log("This is a Delayed task!");
        }, 2.0f); // Delay the task execution by 2 seconds
    }

    public void RecurringTaskExample()
    {
        TaskManager.Instance.ScheduleRecurringTask("RecurringTask", () =>
        {
            Debug.Log("This is a Recurring task!");
        }, 3.0f); // Execute the task every 3 seconds
    }

    public void ContinuationTaskExample()
    {
        int counter = 0;

        TaskManager.Instance.ScheduleContinuationTask("ContinuationTask", () =>
        {
            // Execute the task and return true when the condition is met
            counter++;
            if (counter >= 5)
            {
                Debug.Log("This is a Continuation task, and the condition is met.");
                return true;
            }

            // Task not completed, return false to re-schedule it for the next frame
            Debug.Log("Continuation task is in progress.");
            return false;
        });
    }
}
