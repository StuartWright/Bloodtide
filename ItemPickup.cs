using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IStopped
{
    public Items item;
    //public bool fixedStats;
    private BasePlayer Player;
    private Rigidbody RB;
    private Vector3 Impulse;
    public bool CanImpulse;
    private void Start()
    {
        Player = GameObject.Find("Player").GetComponent<BasePlayer>();
        if(CanImpulse)
        {
            RB = GetComponent<Rigidbody>();
            Impulse = new Vector3(Random.Range(-3, 3), 4, Random.Range(-3, 3));
            RB.AddForce(Impulse, ForceMode.Impulse);
        }       
    }
    public void PlayerStopped()
    {
        Player.GetComponent<IPickup>().PickUp(this);
    }
}


