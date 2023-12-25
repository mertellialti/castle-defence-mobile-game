using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTask
{
    public string UnitName { get; set; }
    public int Amount { get; set; }

    public int CurrentAmount { get; set; }

    // private Dictionary<string, int> killedBy { get; set; }
    public string KilledBy { get; set; }
    public bool IsComplete { get; set; }
    public TaskComponent taskComponent { get; set; }

//  unitName: enemy's name to be killed by player.
//  amount: enemy's amount to be killed by player.
//  Example: unitName = spearMan, amount=5
//  player should kill 5 spearMan in order to complete the task. 
    public LevelTask(string unitName, int amount, TaskComponent tc)
    {
        this.UnitName = unitName;
        this.Amount = amount;
        CurrentAmount = 0;
        IsComplete = false;
        this.taskComponent = tc;
    }

//  killedBy: item's name and amount in order to kill a enemy by player.
//  Example: unitName = axeMan, string killedBy, int amount>
//  player should kill 10 axeMan via "playerArrow".
    public LevelTask(string unitName, string killedBy, int amount, TaskComponent tc)
    {
        this.UnitName = unitName;
        this.KilledBy = killedBy;
        this.Amount = amount;
        CurrentAmount = 0;
        IsComplete = false;
        this.taskComponent = tc;
    }

    public bool CheckTaskDone()
    {
        if (CurrentAmount >= Amount)
        {
            IsComplete = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateTaskComponent()
    {
        if (IsComplete)
        {
            taskComponent.taskText.text = "Completed";
        }
        else
        {
            if (KilledBy == null)
                taskComponent.taskText.text = "Kill " + UnitName + ": " + CurrentAmount + "/" + Amount;
            else
            {
                taskComponent.taskText.text =
                    "Kill " + UnitName + " with " + KilledBy + ": " + CurrentAmount + "/" + Amount;
            }
        }
    }
}