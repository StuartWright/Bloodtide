using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Level1Manager : MonoBehaviour
{
    public static Level1Manager Instance;
    public delegate void DestroyNpc();
    public event DestroyNpc KillRaiders;
    public BaseNPC NPCtoKill;
    public Transform[] SpawnPoints;
    public GameObject Raider;
    public GameObject CommanderCam;
    public List<GameObject> PreSetRaiders;
    public float SpawnTime;
    private GameObject Player;
    public GameObject TargetMarker, AoeCricle, MeleeSkill1Effect;

    public string Tag;
    public GameObject Prefab;
    public int Size;
    public Dictionary<string, Queue<GameObject>> PoolDictionary;
    //public Dialog dialog;
    //private ObjectPooler OP;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {        
        Player = GameObject.Find("Player");
        Player.GetComponent<BasePlayer>().SpawnPoint = new Vector3(32, 6.5f, -10);
        StartCoroutine(Test());
        //Test();
        NPCtoKill.OnDeath += NPCKilled;
        TargetMarker = GameObject.Find("TargetMarker");
        AoeCricle = GameObject.Find("AoeCircle");
        MeleeSkill1Effect = Player.GetComponent<BasePlayer>().MeleeSkill1;
        Player.GetComponent<BasePlayer>().PlayBlackFadeIn();
        DialogManager.Instance.AcceptClicked += StartBattle;

        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        Queue<GameObject> ObjectPool = new Queue<GameObject>();

        for (int i = 0; i < Size; i++)
        {
            GameObject obj = Instantiate(Prefab);
            obj.transform.position = new Vector3(-30,0,0);
            //obj.SetActive(false);
            ObjectPool.Enqueue(obj);
        }
        PoolDictionary.Add(Tag, ObjectPool);


        //OP = GetComponent<ObjectPooler>();       
    }
    public GameObject SpawnFromPool(string tag)
    {
        GameObject ObjectToSpawn = PoolDictionary[tag].Dequeue();

        ObjectToSpawn.SetActive(true);
        //ObjectToSpawn.transform.position = position;

        PoolDictionary[tag].Enqueue(ObjectToSpawn);

        return ObjectToSpawn;
    }
    IEnumerator Test()
    {
        yield return new WaitForSeconds(1);
        Player.gameObject.SetActive(false);
        Player.transform.position = new Vector3(32, 6.5f, -10);
        Player.gameObject.SetActive(true);
    }
    private void NPCKilled(BaseNPC Sender)
    {
        DontDestroyOnLoad(TargetMarker);
        DontDestroyOnLoad(AoeCricle);
        MeleeSkill1Effect.SetActive(true); 
        DontDestroyOnLoad(MeleeSkill1Effect); 
        StopAllCoroutines();
        KillRaiders();
        CommanderCam.SetActive(true);
        DialogManager.Instance.AcceptClicked += EndScene;
        DialogManager.Instance.AcceptClicked += Player.GetComponent<BasePlayer>().PlayBlackFadeOut;
        DialogManager.Instance.AcceptClicked -= StartBattle;
        Player.transform.position = new Vector3(0, 0, 0);
    }
    
    private IEnumerator SpawnRaider()
    {
        yield return new WaitForSeconds(SpawnTime);
        //Instantiate(Raider, SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position, transform.rotation);
        GameObject raider = SpawnFromPool("Raider");
        raider.transform.position = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position;
        StartCoroutine(SpawnRaider());
    }

    private void StartBattle()
    {      
        StartCoroutine(SpawnRaider());
        foreach(GameObject T in PreSetRaiders)
        {
            T.SetActive(true);
        }
    }
    private void EndScene()
    {
        DialogManager.Instance.AcceptClicked -= EndScene;
        DialogManager.Instance.ClosePanel();
        StartCoroutine(SceneChangeDelay());
    }
    IEnumerator SceneChangeDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Level2");
    }
}
