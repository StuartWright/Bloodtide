using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : BasicEnemy
{
    public new event CombatEvents AtTarget;
    public Transform PlayerTarget;
    public float PetTimer;
    private bool PetDead;
    protected override void Start()
    {
        AtTarget += AttackTarget;
        FriendlyList.Add(name);

        base.Start();
    }
    void FixedUpdate()
    {
        if (Target == null)
        {
            agent.SetDestination(PlayerTarget.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Anim.SetBool("Running", true);
                IsAtTarget = false;
            }
            else if (!IsAtTarget && agent.remainingDistance > 0.1f)
            {
                IsAtTarget = true;
                Anim.SetBool("Running", false);
            }
            Hit = Physics.OverlapSphere(transform.position, SpottingDistance, layerMask);
            foreach (Collider target in Hit)
            {
                //if (target.gameObject != this.gameObject && target.tag == "NPC" && !FriendlyList.Contains(target.name) && Target.GetComponent<BaseNPC>().IsEnemy)
                if (target.gameObject != this.gameObject && target.tag == "NPC" && target.gameObject.name != "Player" && target.GetComponent<BaseNPC>().IsEnemy)
                {
                    target.GetComponent<BaseNPC>().HasTarget = true;
                    Target = target.transform;

                    break;
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

    private void Update()
    {
        if(!PetDead)
        {
            PetTimer -= Time.deltaTime;
            if(PetTimer <= 0)
            {
                Health = 0;
            }
        }
    }
    public override void HasDied(BaseNPC Sender)
    {
        PetDead = true;
        base.HasDied(Sender);
    }
}
