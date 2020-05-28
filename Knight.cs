using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Knight : BasicEnemy
{
    public event CombatEvents AtTarget;
    public GameObject UnEquippedHammer, EquippedHammer;       
    public bool ReturnToPosition, IsCommander, IsBoss;
    public Slider HPBar;
    public override float Health
    {
        get { return health; }
        set
        {
            health = value;
            if(IsCommander)
            {
                HPBar.value = Health / MaxHealth;
            }
            base.Health = health;
        }
        
    }
    
    protected override void Start()
    {
        if(!IsBoss)
        {
            AtTarget += AttackTarget;
            FriendlyList.Add(name);            
            if (!IsCommander)
                Level1Manager.Instance.KillRaiders += KillKnights;
        }       
        base.Start();
    }
    void FixedUpdate()
    {
        if (Target == null)
        {
            agent.SetDestination(StartPos);
            if (agent.remainingDistance > agent.stoppingDistance)
                Anim.SetBool("Running", true);
            else
                Anim.SetBool("Running", false);
            Hit = Physics.OverlapSphere(transform.position, SpottingDistance, layerMask);
            foreach (Collider target in Hit)
            {
                if (target.gameObject != this.gameObject && target.tag == "NPC")
                {
                    if (!FriendlyList.Contains(target.name))
                    {
                        ReturnToPosition = false;
                        Target = target.transform;
                        //if (target.name != "Player")
                         //   target.GetComponent<BaseNPC>().OnDeath += EnemyDied;
                        EquippedHammer.SetActive(true);
                        UnEquippedHammer.SetActive(false);
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
    public override void EnemyDied(BaseNPC Sender)
    {
        //Sender.OnDeath -= EnemyDied;
        //if(agent.isOnNavMesh)
        //agent.isStopped = false;
        //Target = null;
        //if (Anim != null)
        //{
        //    Anim.SetBool("Running", false);
        //    Anim.SetBool("Attacking", false);
        //}
        if(this != null)
        {
            EquippedHammer.SetActive(false);
            UnEquippedHammer.SetActive(true);
            IsAtTarget = false;
            ReturnToPosition = true;
            base.EnemyDied(null);
        }
        
    }
    public override void HasDied(BaseNPC Sender)
    {
        NullEnemy();
        Anim.SetBool("Dead", true);
        RB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<BaseNPC>().enabled = false;
        NPCName.gameObject.SetActive(false);
        Level1Manager.Instance.SpawnTime += 0.15f;
        if(!IsCommander)
        StartCoroutine(HideTimer());
    }
    public void KillKnights()
    {
        if(this != null)
        {
            Level1Manager.Instance.KillRaiders -= KillKnights;
            Destroy(gameObject);
        }       
    }
    public override void Clicked() 
    { 
        if(IsCommander || IsBoss)
        {
            Player.CurrentTarget = this;
            TargetMarker.transform.position = gameObject.transform.position;
            TargetMarker.transform.SetParent(gameObject.transform);
        }
    }
}
