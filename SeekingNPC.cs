using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekingNPC : BasicEnemy
{
    public new event CombatEvents AtTarget;
    private RaycastHit hit;
    private Vector3 WalkToPoint;
    bool Roaming;  
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
            Roam();
        }
        else if (Target != null)
        {
            agent.speed = RunSpeed;
            agent.SetDestination(Target.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Anim.SetBool(RoamingAnim, false);
                Anim.SetBool("Running", true);
                IsAtTarget = false;
                float Distance = Vector3.Distance(transform.position, Target.position);
                if (Distance > 20)
                    EnemyDied(null);
            }
            else if (!IsAtTarget && agent.remainingDistance > 0.1f)
            {
                IsAtTarget = true;
                transform.LookAt(Target);
                if(Ranged)
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + 120, 0);
                Anim.SetBool("Running", false);
                AtTarget();
            }
        }
    }
    private void Roam()
    {
        agent.speed = WalkSpeed;
        if(Roaming)
        {
            agent.SetDestination(WalkToPoint);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Anim.SetBool(RoamingAnim, true);
            }
            else if (agent.remainingDistance > 0.1f)
            {
                Anim.SetBool(RoamingAnim, false);
                Roaming = false;
            }
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
        else
        {            
            if (Physics.Raycast(Spawner.Pos() + new Vector3(0, 10, 0), Vector3.down, out hit, 11))
            {
                
                if (hit.collider.name == "Terrain")
                {
                    WalkToPoint = hit.point;
                    Roaming = true;
                }
            }
        }              
    }
}
