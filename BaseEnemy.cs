using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseEnemy : BaseNPC
{   
    public event CombatEvents AtTarget;   
    public int SpottingDistance;
    public Collider[] Hit;
    int layerMask = 1 << 8; //Layer 8

    protected new void Start()
    {
        AtTarget += AttackPlayer;
        base.Start();
    }

    private void FixedUpdate()
    {
        if (Target == null)
        {
            Hit = Physics.OverlapSphere(transform.position, 10, layerMask);
            foreach (Collider target in Hit)
            {
                if (target.tag == "NPC" && IsEnemy)
                {
                    Target = target.transform;
                }
            }
        }
        else if (Target != null)
        {
            agent.SetDestination(Target.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                agent.isStopped = false;
                Anim.SetBool("Running", true);
            }
            else
            {
                agent.isStopped = true;
                Anim.SetBool("Running", false);
                AtTarget();
            }
        }
    }


    public void AttackPlayer()
    {
        Anim.SetBool("Attacking", true);
    }
    public void DealDamage()
    {
        Target.GetComponent<IDamagable>().TakeDamage(Damage, null, false);
    }
    public void StopAnim()
    {
        Anim.SetBool("Attacking", false);
        if (Player != null && agent.remainingDistance <= agent.stoppingDistance)
        {
            AttackPlayer();
        }
    }
}
