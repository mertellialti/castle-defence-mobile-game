using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneTaskManager : Singleton<SceneTaskManager>
{
    public Dictionary<string, List<LevelTask>> Tasks { get; set; }
    public LevelUIController UIController { get; set; }
    public bool isLevelVictory = false;
    public void ResetDictionary()
    {
        Tasks = new Dictionary<string, List<LevelTask>>();
    }
    public void UpdateTask(EnemyData enemyData)
    {
        Debug.Log("Update Task Enemy Retrieved: " + enemyData.EnemyName);
        if (Tasks.ContainsKey(enemyData.EnemyName))
        {
            Debug.Log("task list contains " + enemyData.EnemyName);

            List<LevelTask> tList = Tasks[enemyData.EnemyName];

            foreach (LevelTask t in tList)
            {
                if (!t.IsComplete)
                {
                    if (t.KilledBy != null)
                    {
                        Debug.Log("killed by " + enemyData.KilledByName + " should killed by: " + t.KilledBy);
                        if (t.KilledBy.Equals(enemyData.KilledByName))
                        {
                            t.CurrentAmount++;
                        }
                    }
                    else
                    {
                        Debug.Log("amount by task");
                        t.CurrentAmount++;
                    }
                    t.UpdateTaskComponent();
                    var status = t.CheckTaskDone();
                    if (status)
                    {
                        Debug.Log("A Task Done!");
                    }
                }
            }
            PrintTasksStatus();
            if (IsAllTasksDone())
            {
                isLevelVictory = true;
                Debug.LogWarning("All tasks done, Level Victory!");
                //Hide panels and stuff...
                SceneEnemiesManager.Instance.RetreatAllEnemies();
                UIController.HideAll();
                Time.timeScale = 0.2f;
            }
        }
        else
        {
            Debug.LogError("enemy does not included in this levels tasks");
        }
    }
    // checks all the tasks. If any task is incomplete, returns false. 
    // if all tasks are completed, returns true.
    private bool IsAllTasksDone()
    {
        foreach (var pair in Tasks)
        {
            foreach (var t in pair.Value)
            {
                if (!t.IsComplete)
                    return false;
            }
        }
        return true;
    }
    public void PrintTasksStatus()
    {
        for (int i = 0; i < Tasks.Count; i++)
        {
            var l = Tasks.ElementAt(i).Value;
            for (int j = 0; j < l.Count; j++)
            {
                Debug.Log("TASK--> " + Tasks.Keys.ElementAt(i) + "\n" + (j + 1) + ". " + l[j].UnitName + " " +
                                 l[j].CurrentAmount + "/" + l[j].Amount);
            }
        }
    }
    
    
        #region OldCodeRegion

    
// public Dictionary<string, int> KillTask { get; set; }
    // public bool IsVictory { get; set; }

    // public void CreateTaskPanelForScene(Dictionary<string, int> dictionary)
    // {
    //     KillTask = dictionary;
    //     foreach (var task in KillTask)
    //     {
    //         var component = Instantiate(taskUIComponentPrefab);
    //         var data = component.GetComponent<TaskComponent>();
    //         data.TaskKey = task.Key;
    //         data.CurrentVal = task.Value;
    //         data.MaxVal = task.Value;
    //         data.taskText.text = task.Key + ": " + (data.MaxVal - data.CurrentVal) + "/" + data.MaxVal;
    //         component.transform.SetParent(taskContainer);
    //     }
    // }
    //
    // public void UpdateTaskDictionaryComponents(string deathEnemyName)
    // {
    //     if (KillTask.ContainsKey(deathEnemyName))
    //     {
    //         KillTask[deathEnemyName]--;
    //         if (CheckVictoryCondition())
    //         {
    //             UpdateTaskPanelComponents(deathEnemyName);
    //             Debug.LogWarning("VICTORY!");
    //             //retreat all enemies to back!
    //             //end the scene later. 
    //             //for now, just freeze the game!
    //             Time.timeScale = 0;
    //         }
    //         else
    //             UpdateTaskPanelComponents(deathEnemyName);
    //     }
    // }
    //
    // private void UpdateTaskPanelComponents(string deathEnemyName)
    // {
    //     if (KillTask.ContainsKey(deathEnemyName))
    //     {
    //         for (int i = 0; i < taskContainer.childCount; i++)
    //         {
    //             var comp = taskContainer.GetChild(i);
    //             var data = comp.gameObject.GetComponent<TaskComponent>();
    //             if (data.TaskKey.Equals(deathEnemyName))
    //             {
    //                 if (data.MaxVal - KillTask[deathEnemyName] > data.MaxVal)
    //                 {
    //                     var current = data.MaxVal;
    //                     data.taskText.text = deathEnemyName + ": " + (current) + "/" +
    //                                          data.MaxVal;
    //                 }
    //                 else
    //                 {
    //                     var current = data.MaxVal - KillTask[deathEnemyName];
    //                     data.taskText.text = deathEnemyName + ": " + (current) + "/" +
    //                                          data.MaxVal;
    //                 }
    //                 // data.taskText.text = deathEnemyName + ": " + (data.MaxVal - KillTask[deathEnemyName]) + "/" +
    //                 //                      data.MaxVal;
    //                 return;
    //             }
    //         }
    //     }
    // }
    //
    // private bool CheckVictoryCondition()
    // {
    //     for (int i = 0; i < KillTask.Count; i++)
    //     {
    //         Debug.Log("enemy " + KillTask.ElementAt(i).Key + " left " + KillTask.ElementAt(i).Value);
    //         if (KillTask.ElementAt(i).Value > 0)
    //             return false;
    //     }
    //     victoryPanel.gameObject.SetActive(true);
    //     taskContainer.parent.gameObject.SetActive(false);
    //     return true;
    // }
    //
    #endregion
}