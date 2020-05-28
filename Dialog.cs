using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog 
{
    //[TextArea(3, 10)]
    public string QuestText;
    public string CompletedQuestText;
    public string FailedQuestText;
    public string[] NormalText;
    private Queue<string> normalText = new Queue<string>();
    public BaseNPC Parent;
    public bool NoButton;
    
    public string WhatToSay()
    {
        if(!NoButton)
        {
            DialogManager.Instance.LeaveButton.SetActive(false);
        }
        else
            DialogManager.Instance.LeaveButton.SetActive(true);
        DialogManager.Instance.AcceptButton.gameObject.SetActive(true);
        SpeechState State = Parent.speechState;
        switch (State)
        {
            case SpeechState.QivingQuest:
                return QuestText;
            case SpeechState.CompletedQuest:
                return CompletedQuestText;
            case SpeechState.QuestFailed:
                return FailedQuestText;
            case SpeechState.Normal:
                DialogManager.Instance.AcceptButton.gameObject.SetActive(false);
                foreach (string Sentence in NormalText)
                {
                    normalText.Enqueue(Sentence);
                }
                return normalText.Dequeue();
        }
        return null;
    }
}
