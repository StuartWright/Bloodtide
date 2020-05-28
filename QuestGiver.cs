using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestGiver : MonoBehaviour
{
    public Quest QuestToGive;
    //public bool HasStartedChat;
    public Image QuestImage;
    public Sprite AvaliableQuest, ActivatedQuest, CompletedQuest;
    private Dialog dialog;
    private BasePlayer Player;
    public bool HasOpend, ReOpend;
    private void Start()
    {
        if(!QuestToGive.StoryLine)
        dialog = GetComponent<BaseNPC>().dialog;
        Player = GameObject.Find("Player").GetComponent<BasePlayer>();
    }
    public void PlayerStopped()
    {        
        if (!QuestToGive.IsActive && !QuestToGive.IsComplete)
        {
            DialogManager.Instance.UIText(dialog);
            DialogManager.Instance.AcceptClicked += AcceptQuest;
            DialogManager.Instance.CloseClicked += RemoveListener;
        }  
        else if(QuestToGive.ReadyToBeCompleted)
        {
            DialogManager.Instance.UIText(dialog);
            DialogManager.Instance.AcceptClicked += QuestToGive.CompleteQuest;
            DialogManager.Instance.CloseClicked += RemoveRewardListener;
            
        }
    }

    public void RemoveListener()
    {
        DialogManager.Instance.AcceptClicked -= AcceptQuest;
        DialogManager.Instance.CloseClicked -= RemoveListener;
    }
    public void RemoveRewardListener()
    {
        DialogManager.Instance.AcceptClicked -= QuestToGive.CompleteQuest;
        DialogManager.Instance.CloseClicked -= RemoveRewardListener;
    }
    private void AcceptQuest()
    {
        DialogManager.Instance.AcceptClicked -= AcceptQuest;       
        QuestToGive.Giver = this;
        QuestToGive.IsActive = true;
        QuestToGive.Player = Player;
        Player.Quests.Add(QuestToGive);
        Player.QuestTextParent.SetActive(true);
        QuestImage.sprite = ActivatedQuest;
        QuestToGive.ActivateQuest();
        if (QuestToGive.QuestName == "Save my farm")
        {
            GameObject.Find("GolemSpawner").GetComponent<Spawner>().DestroySpawner();
        }
            
        DialogManager.Instance.ClosePanel();
    }
}
