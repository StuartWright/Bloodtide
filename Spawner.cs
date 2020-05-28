using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Mob;
    public List<GameObject> Spawnies = new List<GameObject>();
    private int CurrentMobs;
    public int MaxMobs;
    public float Range;
    private float Distance;
    private GameObject Player;
    private Transform child;
    private RaycastHit Hit;
    void Start()
    {
        CurrentMobs = Spawnies.Count;
        foreach(GameObject mobs in Spawnies)
        {
            mobs.GetComponent<BaseNPC>().OnDeath += MobDied;
        }
        Player = GameObject.Find("Player");
        child = transform.Find("MobParent");
        StartCoroutine(SpawnMob());
    }
    private void LateUpdate()
    {
        Distance = Vector3.Distance(transform.position, Player.transform.position);
        if (Distance > 80)
            child.gameObject.SetActive(false);
        else
            child.gameObject.SetActive(true);
    }
    private void MobDied(BaseNPC Sender)
    {
        Sender.OnDeath -= MobDied;
        Spawnies.Remove(Sender.gameObject);
        CurrentMobs--;
        if(this != null)
        StartCoroutine(SpawnMob());
    }
    public IEnumerator SpawnMob()
    {
        yield return new WaitForSeconds(5);
        Spawn();
    }

    private void Spawn() 
    {
        if (CurrentMobs < MaxMobs)
        {
            //for(int i = 0; i < 10; i++)
            //{
            Vector3 pos = Pos();
                if (Physics.Raycast(pos + new Vector3(0, 10, 0), Vector3.down, out Hit, 12))
                {
                if (Hit.collider.name == "Terrain")
                {
                    GameObject NewMob = Instantiate(Mob, pos, transform.rotation);
                    NewMob.transform.SetParent(child);
                    Spawnies.Add(NewMob);
                    NewMob.GetComponent<BaseNPC>().OnDeath += MobDied;
                    NewMob.GetComponent<BaseNPC>().Spawner = this;
                    CurrentMobs++;
                    StartCoroutine(SpawnMob());
                    return;
                }
                else
                    Spawn();
                }
            //}                                  
        }      
    }
    public void DestroySpawner()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
    public Vector3 Pos()
    {
        return transform.position + new Vector3(Random.Range(-Range, Range), 0, Random.Range(-Range, Range));
    }
}
