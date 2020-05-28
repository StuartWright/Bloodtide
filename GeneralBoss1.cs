using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GeneralBoss1 : Knight
{
    public new event CombatEvents AtTarget;
    public GameObject BridgeBlock;
    public ParticleSystem AoeCircle;
    private int AOEValue = 5;
    private bool AOE = true;
    private float AOETimer = 15;
    protected override void Start()
    {
        AtTarget += AttackTarget;
        OnDeath += Level2Manager.Instance.CommanderDead;
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
                if (target.gameObject != this.gameObject && target.tag == "NPC" && !FriendlyList.Contains(target.name))
                {
                    if (!FriendlyList.Contains(target.name))
                    {
                        ReturnToPosition = false;
                        Target = target.transform;
                        EquippedHammer.SetActive(true);
                        UnEquippedHammer.SetActive(false);
                        break;
                    }
                }
            }
        }
        else if (Target != null)
        {
            if(!IsAtTarget)
            agent.SetDestination(Target.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Anim.SetBool("Running", true);
                IsAtTarget = false;
                float Distance = Vector3.Distance(transform.position, Target.position);
                if (Distance > 25)
                    EnemyDied(null);
            }
            else if (!IsAtTarget && agent.remainingDistance > 0.1f)
            {
                IsAtTarget = true;
                Anim.SetBool("Running", false);
                AtTarget();
            }
        }
    }

    public override void StopAnim()
    {
        if (agent.isOnNavMesh)
        {
            Anim.SetBool("Attacking", false);
            if(AOE)
            {
                IsAtTarget = true;
                Anim.SetBool("AOESpell", true);
            }
            else if (Target != null && agent.remainingDistance <= agent.stoppingDistance)
            {
                AttackTarget();
            }
        }
        IsAtTarget = false;
    }
    private void LateUpdate()
    {
        if(!AOE)
        {
            AOETimer -= Time.deltaTime;
            if(AOETimer <= 0)
            {
                AOE = true;
                AOETimer = 15;
            }
        }
    }
    public void AoeSpell()
    {
        AOE = false;
        AoeCircle.Play();
        Collider[] Hit;
        Hit = Physics.OverlapSphere(transform.position, AOEValue);
        foreach (Collider hit in Hit)
        {
            if (hit.gameObject != this.gameObject && hit.GetComponent<IDamagable>() != null)
            {
                hit.GetComponent<IDamagable>().TakeDamage(Damage, transform.transform, false);
            }
        }
    }

    public void EndAOESpell()
    {
        Anim.SetBool("AOESpell", false);
        IsAtTarget = false;
        if (Target != null)
        {
            StopAnim();
        }
    }

    public void GetDistance()
    {
        if (Target != null)
        {
            agent.SetDestination(Target.position);
        }
    }
    public override void HasDied(BaseNPC Sender)
    {
        NullEnemy();
        Destroy(BridgeBlock);
        Player.SetPopUpUi(ExpToGive, true);
        int RandomNum = Random.Range(0, 100);
        if (RandomNum <= 30)
        {
            Vector3 Pos = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            Instantiate(ItemManager.instance.RandomItem(), transform.position + Pos, Quaternion.Euler(new Vector3(-90, 10, 0)));
        }
        Anim.SetBool("Running", false);
        Anim.SetBool("Attacking", false);
        Anim.SetBool("Dead", true);
        if (FreezeGO != null)
            Destroy(FreezeGO);
        RB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<BaseNPC>().enabled = false;
        NPCName.gameObject.SetActive(false);
        StartCoroutine(HideTimer());
    }
}
