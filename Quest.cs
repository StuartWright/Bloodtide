using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//public struct Quests { public float xp; }
public enum QuestType
{
    KillTarget,
    Collect,
    Protect,
    Dual
}
[System.Serializable]
public class Quest
{
    public delegate void QuestEvent(Items item, Quest quest);
    public event QuestEvent RemoveQuestItem;
    public Text QuestTitle, QuestDescription;
    public bool IsComplete;
    public bool IsActive, StoryLine;
    public string QuestName;
    //public string QuestDescription;
    public string CompletedDescription, StoryDescription;
    //public QuestTracker CurrentQuest;
    public BaseNPC Target;
    public QuestType Type;
    public BasePlayer Player;
    public QuestGiver Giver;
    public bool ReadyToBeCompleted;
    public int Exp;
    public Items CollectionItem;
    private int currentAmount;
    public int CurrentAmount
    {
        get { return currentAmount; }
        set
        {
            currentAmount = value;
            if (IsReached())
            {
                if(StoryLine)
                {
                    CompleteQuest();
                    return;
                }
                Giver.GetComponent<BaseNPC>().speechState = SpeechState.CompletedQuest;
                ReadyToBeCompleted = true;
                QuestDescription.text = Description();
                Giver.QuestImage.sprite = Giver.CompletedQuest;
                if (Type == QuestType.Dual)
                {
                    Player.CurrentTarget = null;
                    Giver.PlayerStopped();
                }
            }
        }
    }
    public int RequiredAmount;

    public void QuestRewards()
    {
        Player.SetPopUpUi(Exp, true);
        IsComplete = true;
        IsActive = false;
    }
    public bool IsReached()
    {
        //if(Type == QuestType.KillTarget || Type == QuestType.Dual)
        //{
            if (CurrentAmount >= RequiredAmount)
                return true;
            else
          {
              QuestDescription.text = Description();
          }
        //}
        return false;
    }
    public string Description()
    {
        if (ReadyToBeCompleted)
        {
            return CompletedDescription;
        }
        if(StoryLine)
            return StoryDescription;
        switch (Type)
        {
            case QuestType.KillTarget:
                return "Kill " + CurrentAmount + "/" + RequiredAmount + " " + Target.Name;
            case QuestType.Protect:
                return "Protect " + Target.Name + " from being killed";
            case QuestType.Collect:
                return "Collect " + CurrentAmount + "/" + RequiredAmount + " " + CollectionItem.ItemName;
                
        }        
        return "";
    }
    public void ActivateQuest()
    {
        SetText();
        switch (Type)
        {
            case QuestType.Protect:
                Target.OnDeath += FailedQuest;
                break;
            case QuestType.Dual:
                //Target.Defeat += CompleteQuest;
                Target.TurnEnemy();
                Player.PlayerDied += FailedQuest;
                break;
            case QuestType.Collect:
                Player.inventory.CheckQuestItem(CollectionItem, this);
                RemoveQuestItem += Player.inventory.RemoveQuestItem;
                break;
        }
    }
    public void SetText()
    {
        if(Player.Quests.Count > 0)
        {
            QuestTitle = Player.QuestTitles[Player.Quests.IndexOf(this)];
            QuestDescription = Player.QuestDesctiptions[Player.Quests.IndexOf(this)];
            QuestTitle.text = QuestName;
            QuestDescription.text = Description();
        }     
    }
    public void NullText()
    {
        QuestTitle.text = "";
        QuestDescription.text = "";
        QuestTitle = null;
        QuestDescription = null;
    }
        

    public void FailedQuest(BaseNPC sender)
    {
        Giver.QuestImage.enabled = false;
        IsActive = false;
        Player.NullQuestTexts();
        if (Type == QuestType.Dual)
        {
            Player.PlayerDied -= FailedQuest;
            Target.RevertEnemy();
            Giver.HasOpend = false;
            
            Giver.QuestImage.enabled = true;
            Giver.QuestImage.sprite = Giver.AvaliableQuest;
        }
        else if(Type != QuestType.Dual)
        {
            Giver.GetComponent<BaseNPC>().speechState = SpeechState.QuestFailed;
            DialogManager.Instance.UIText(Giver.GetComponent<BaseNPC>().dialog);
        }
        Player.Quests.Remove(this);
        Player.UpdateList();
          
        //DialogManager.Instance.AcceptButton.gameObject.SetActive(false);
    }
    public void CompleteQuest()
    {
        if(!StoryLine)
        {
            DialogManager.Instance.AcceptClicked -= CompleteQuest;          
            if (Type == QuestType.Collect)
            {
                RemoveQuestItem(CollectionItem, this);
            }
            ReadyToBeCompleted = false;
            Giver.QuestImage.enabled = false;
            DialogManager.Instance.ClosePanel();
        }
        QuestRewards();
        Player.NullQuestTexts();
        Player.Quests.Remove(this);
        Player.UpdateList();
        
    }
}