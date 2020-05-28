using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public enum QuestType
{
    KillTarget,
    Escort
}
*/
[System.Serializable]
public class QuestTracker
{
    public BaseNPC Target;
    public QuestType Type;
    public BasePlayer Player;
    public bool ReadyToBeCompleted;
    private int currentAmount;
    public int CurrentAmount
    {
        get { return currentAmount; }
        set
        {
            currentAmount = value;
            if (IsReached())
                ReadyToBeCompleted = true;
        }
    }
    public int RequiredAmount;
    public void ActivateQuest()
    {
        if(Type == QuestType.KillTarget)
        {
            Target.OnDeath += UpdateKillQuest;
        }
    }
    public bool IsReached()
    {
        if (CurrentAmount >= RequiredAmount)
            return true;
        else
            return false;
    }
    public void UpdateKillQuest(BaseNPC Sender)
    {
        if(Sender == Target.GetComponent<BaseNPC>())
        {
            CurrentAmount++;
        }      
    }
    public void CompleteQuest()
    {
        //Player.Quest.QuestRewards();
       // Player.Quests.Remove(this);
        //Target.OnDeath -= UpdateKillQuest;
    }
}

