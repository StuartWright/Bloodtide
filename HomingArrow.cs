using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingArrow : MonoBehaviour
{
    private Rigidbody RB;
    public Transform Target, Sender;
    public float Damage, Value;
    public bool Crit, Freeze;
    private bool ChargingDone;
    private int layerMask = 1 << 8; //Layer 8
    private BaseNPC npc;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        StartCoroutine(ChargeTimer());
        StartCoroutine(KillTimer());
    }


    void Update()
    {
        if(!ChargingDone)
        {
            RB.velocity = transform.forward * 5;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1.5f);
            return;
        }
        if(!Target)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8, layerMask);
            float CurrentDistance = Mathf.Infinity; 
            foreach(Collider col in hitColliders)
            {
                if(col.gameObject.name != "Player" && col.GetComponent<BaseNPC>().IsEnemy)
                {
                    float Distance = Vector3.Distance(transform.position, col.transform.position);
                    if(Distance < CurrentDistance)
                    {
                        CurrentDistance = Distance;
                        Target = col.transform;
                        npc = col.GetComponent<BaseNPC>();
                    }                   
                }
            }
            if(Target != null)
            {
                npc.OnDeath += TargetDied;
            }
        }
        if (Target)
        {
            RB.velocity = transform.forward * 10;
            Vector3 dir = Target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 6);

            //float Distance = Vector3.Distance(transform.position, Target.position);
            //if (Distance <= 0.1f)
               // Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Target != null && other.transform == Target.transform)
        {
            Target.GetComponent<IDamagable>().TakeDamage(Damage, Sender, Crit);
            Destroy(gameObject);
        }
    }
    public void TargetDied(BaseNPC Sender)
    {
        Sender.OnDeath -= TargetDied;
        Target = null;
    }
    IEnumerator ChargeTimer()
    {
        yield return new WaitForSeconds(0.6f);
        ChargingDone = true;
    }
    IEnumerator KillTimer()
    {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }
}
