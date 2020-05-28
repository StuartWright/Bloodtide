using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
public class Raider : BasicEnemy
{
    public new event CombatEvents AtTarget;    
    public Transform Commander;
    public bool PreEnemy;
    protected new void Start()
    {
        AtTarget += AttackTarget;
        FriendlyList.Add(name);
        Commander = Level1Manager.Instance.NPCtoKill.transform;
        Level1Manager.Instance.KillRaiders += KillRaider;
        base.Start();
        if (!PreEnemy)
            gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        Health = MaxHealth;
    }
    void FixedUpdate()
    {
        if(IsFrozen)
        {          
            return;
        }
        if (Target == null)
        {
            agent.SetDestination(Commander.position);
            Hit = Physics.OverlapSphere(transform.position, SpottingDistance, layerMask);
            foreach (Collider target in Hit)
            {
                if (target.gameObject != this.gameObject && target.tag == "NPC" && !FriendlyList.Contains(target.name))
                {
                    if (target.gameObject.name != "Player" && !target.GetComponent<BaseNPC>().HasTarget || target.name == "Commander")
                    {
                        target.GetComponent<BaseNPC>().HasTarget = true;
                        Target = target.transform;
 
                        break;
                    }
                }
            }
        }
        else if (Target != null)
        {
            agent.SetDestination(Target.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                //agent.isStopped = false;
                Anim.SetBool("Running", true);
                IsAtTarget = false;
            }
            else if (!IsAtTarget && agent.remainingDistance > 0.1f)
            {
                IsAtTarget = true;
                //agent.isStopped = true;
                Anim.SetBool("Running", false);
                //transform.LookAt(Target);
                AtTarget();
            }
        }
    }
    public new void EnemyDied(BaseNPC Sender)
    {
        //Sender.OnDeath -= EnemyDied;
        //if (agent != null)
        //agent.isStopped = false;
        //Target = null;
        if(Anim != null)
        {
            Anim.SetBool("Running", true);
            Anim.SetBool("Attacking", false);
        }      
        NullEnemy();
        IsAtTarget = false;
    }
    protected override void RemoveEnemy()
    {
        if (PreEnemy)
            Destroy(gameObject);
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<BaseNPC>().enabled = true;
        NPCName.gameObject.SetActive(true);
        gameObject.SetActive(false);
        Anim.SetBool("Dead", false);
    }
    public void KillRaider()
    {
        if(this != null)
        {
            Level1Manager.Instance.KillRaiders -= KillRaider;
            gameObject.SetActive(false);
        }
           
    }
}
