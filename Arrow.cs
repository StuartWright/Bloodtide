using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody RB;
    public Transform Target, Sender;
    public float Damage, Value;
    public bool Crit, Freeze, SkillArrow;
    private BaseNPC npc;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        if (Target.GetComponent<BaseNPC>())
            npc = Target.GetComponent<BaseNPC>();
    }

    
    void Update()
    {
        if (Target)
        {
            RB.transform.position = Vector3.MoveTowards(transform.position, Target.position + new Vector3(0, 1, 0), 10 * Time.deltaTime);            
            RB.transform.LookAt(Target.position + new Vector3(0, 1, 0));
        }
        else
            TargetDied(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == Target.transform)
        {
            if (Freeze)
                Target.GetComponent<IFreezable>().Freeze(Value);
            Target.GetComponent<IDamagable>().TakeDamage(Damage, Sender, Crit);
            if (SkillArrow)
            {
                //Rigidbody rb = Target.GetComponent<Rigidbody>();
                //StartCoroutine(npc.PushBackEnd());
                npc.StartPushBack(Value);
                //Vector3 moveDirection = Target.transform.position - Sender.transform.position;
                //Target.transform.position += moveDirection * 1;
            }
            
            TargetDied(null);
        }            
    }
    public void TargetDied(BaseNPC Sender)
    {
        if (npc)
            npc.OnDeath -= TargetDied;
        Destroy(gameObject);
    }
}
