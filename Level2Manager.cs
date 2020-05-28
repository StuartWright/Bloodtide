using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Manager : MonoBehaviour
{
    public static Level2Manager Instance;
    public BasePlayer Player;
    public Gregory Friend;
    public List<Quest> quests = new List<Quest>();
    private void Awake()
    {
        Player = GameObject.Find("Player").GetComponent<BasePlayer>();
    }
    void Start()
    {
        Instance = this;
        StartCoroutine(SetPosition());
        Player.GetComponent<PlayerController>().SetTarget(Friend.transform);
        Player.GetComponent<PlayerController>().agent.stoppingDistance = 2;
        Player.SpawnPoint = new Vector3(-10, 0, -30);
        Player.GetComponent<BasePlayer>().PlayBlackFadeIn();
        StartCoroutine(SetPosition());
    }
    IEnumerator SetPosition()
    {
        Player.gameObject.SetActive(false);
        Player.gameObject.transform.position = new Vector3(-10, 0, -30);
        Player.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Friend.GetComponent<IClickable>().Clicked();
        Player.GetComponent<PlayerController>().SetTarget(Friend.transform);
    }
    public void CommanderDead(BaseNPC Sender)
    {
        Sender.OnDeath -= CommanderDead;
        Friend.gameObject.transform.position = new Vector3(46.6f, 1.8f, 86);
        Friend.RunToPoint.transform.position = new Vector3(98, 0, 118);
        Friend.gameObject.SetActive(true);       
        Friend.CommanderDead();       
    }
    public void GiveStoryQuest()
    {
        Player.Quests.Add(quests[0]);
        quests[0].Player = Player;
        quests[0].ActivateQuest();
        quests.Remove(quests[0]);
    }
}
    
