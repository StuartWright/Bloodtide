using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.SceneUtils;

public class Gregory : BasicEnemy
{
    public new event CombatEvents AtTarget;
    public GameObject RunToPoint;
    protected override void Start()
    {
        AtTarget += AtPoint;
        FriendlyList.Add(name);
        DialogManager.Instance.CloseClicked += RunAway;
        base.Start();
    }
    void FixedUpdate()
    {
        /*
        if (Target == null)
        {
            Hit = Physics.OverlapSphere(transform.position, SpottingDistance, layerMask);
            foreach (Collider target in Hit)
            {
                if (target.gameObject != this.gameObject && target.tag == "NPC" && !FriendlyList.Contains(target.name))
                {
                    if (!FriendlyList.Contains(target.name))
                    {
                        Target = target.transform;
                        break;
                    }
                }
            }
        }
        */
        if (Target != null)
        {
            agent.SetDestination(Target.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Anim.SetBool("Running", true);
                IsAtTarget = false;
            }
            else if (!IsAtTarget && agent.remainingDistance > 0.1f)
            {
                IsAtTarget = true;
                Anim.SetBool("Running", false);
                AtTarget();
            }
        }       
    }

    private void RunAway()
    {
        AtTarget += AtPoint;
        DialogManager.Instance.CloseClicked -= RunAway;
        Level2Manager.Instance.GiveStoryQuest();
        Target = RunToPoint.transform;
    }
    public void CommanderDead()
    {       
        Target = Player.transform;
        AtTarget += TalkToPlayer;
        DialogManager.Instance.CloseClicked += RunAway;
        RunToPoint.SetActive(true);       
    }
    public void TalkToPlayer()
    {
        AtTarget -= TalkToPlayer;        
        PlaceTargetWithMouse.Instance.setTargetOn.agent.stoppingDistance = 2;
        Clicked();
        PlayerController.SetTarget(transform);
    }
    private void AtPoint()
    {
        AtTarget -= AtPoint;
        if (Player.CurrentTarget == this)
        {
            TargetMarker.transform.position = new Vector3(0, -1, 0);
            TargetMarker.transform.parent = null;
        }
        Target = null;
        RunToPoint.SetActive(false);
        gameObject.SetActive(false);
    }
}
