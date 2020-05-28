using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : BaseNPC
{
    protected event CombatEvents AtTarget;    
    public bool Ranged;
    public Collider[] Hit;
    public int SpottingDistance;
    protected int layerMask = 1 << 8; //Layer 8
    public List<string> FriendlyList = new List<string>();
    public GameObject Arrow;
    
    public Transform BowShootPoint;
    protected override void Start()
    {
        AtTarget += AttackTarget;
        StartPos = transform.position;
        base.Start();
    }
    
     void FixedUpdate()
    {
        if(ReturnToPosition)
        {           
            if (agent.remainingDistance > agent.stoppingDistance)
                Anim.SetBool("Running", true);
            else
            {
                ReturnToPosition = false;
                Anim.SetBool("Running", false);
            }               
        }
        if (Target != null)
        {
            agent.SetDestination(Target.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Anim.SetBool("Running", true);
                IsAtTarget = false;
            }
            else if(!IsAtTarget)
            {
                IsAtTarget = true;
                Anim.SetBool("Running", false);
                AtTarget();
            }
        }
    }
    public void AttackTarget()
    {
        Anim.SetBool("Attacking", true);
    }
    public void DealDamage()
    {
        if (Target != null)
            Target.GetComponent<IDamagable>().TakeDamage(Damage, gameObject.transform, false);
    }
    public virtual void StopAnim()
    {
        if(agent.isOnNavMesh)
        {
            Anim.SetBool("Attacking", false);
            if (Target != null && agent.remainingDistance <= agent.stoppingDistance)
            {
                AttackTarget();
            }
        }        
    }
    public void FireArrow()
    {
        if (Target)
        {
            Arrow arrow = Instantiate(Arrow, BowShootPoint.transform.position, transform.rotation).GetComponent<Arrow>();
            arrow.Target = Target.transform;
            //Target.OnDeath += arrow.TargetDied;
            arrow.Sender = this.transform;
            arrow.Damage = Damage;
        }
    }
}
