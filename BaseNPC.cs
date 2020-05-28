using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public enum SpeechState
{ 
    QivingQuest,
    CompletedQuest,
    QuestFailed,
    Normal

}

public class BaseNPC : MonoBehaviour, IDamagable, IClickable, IStopped, IFreezable
{
    public delegate void CombatEvents();
    public delegate void Events(BaseNPC Sender);
    public event Events OnDeath;
    public event CombatEvents Defeat;
    protected NavMeshAgent agent;
    public Rigidbody RB;
    [SerializeField]
    private Transform target;
    public Transform Target
    {
        get { return target; }
        set
        {
            target = value;
            if (Target != null)
            {
                HasTarget = true;
                if (Target.gameObject.name != "Player" && Target.GetComponent<BaseNPC>())
                    Target.GetComponent<BaseNPC>().OnDeath += EnemyDied;
                else if (Target.gameObject.name == "Player")
                {
                    Player.PlayerDied += EnemyDied;
                    Player.PlayerDied += EnableEnemyRun;                        
                }                    
            }
            else
            {
                HasTarget = false;                
            }                
        }
    }
    public bool HasTarget;
    private bool TurnedEnemy, PushBack, Regen;
    protected BasePlayer Player;
    protected PlayerController PlayerController;
    protected Animator Anim;
    public GameObject Canvas;
    public Text NPCName;
    public float PlayerStopDistance;
    public string Name;
    public bool IsEnemy, DontRegen;
    protected bool IsAtTarget, ReturnToPosition;
    protected bool PlayerLastHitter;
    protected bool IsFrozen;   
    public int Damage;
    public int ExpToGive;
    public int PushBackForce;
    public int Level;
    public ParticleSystem HitEffect;
    protected Vector3 StartPos;
    protected string RoamingAnim;
    public SpeechState speechState;
    public Spawner Spawner;
    public bool HasWalkAnim;
    public float RunSpeed, WalkSpeed, ThisMobLootChance;
    protected GameObject TargetMarker, FreezeGO;
    public GameObject FreezeParticle, ThisMobLoot;
    protected float DamageFromPlayer;
    public float MaxHealth;
    [SerializeField]
    protected float health;
     public virtual float Health
    {
        get { return health; }
        set
        {
            health = Mathf.Max(Mathf.Min(value, MaxHealth), 0);
            if (Player.CurrentTarget == this)
            {
                Player.EnemyHealth = Health;
            }
            if (Health <= 0)
            {
                if(TurnedEnemy)
                {
                    Player.UpdateKillQuest(this);
                    Defeat();
                    return;
                }
                if(Player.CurrentTarget == this)
                {
                    TargetMarker.transform.position = new Vector3(0, -1, 0);
                    TargetMarker.transform.parent = null;
                    Player.CurrentTarget = null;
                }
                
                if (Player.Quests.Count >0)
                    Player.UpdateKillQuest(this);
                OnDeath(this);               
            }
        }
    }

    private IEnumerator HealthRegen()
    {
        yield return new WaitForSeconds(2);
        if (Health <= MaxHealth)
            Health += 1;
        if(Health > MaxHealth)
        {
            Health = MaxHealth;
            Regen = false;
        }
            
        if(Regen)
        StartCoroutine(HealthRegen());
    }
    public Dialog dialog;
    private void Awake()
    {
        Player = GameObject.Find("Player").GetComponent<BasePlayer>();
    }
    protected virtual void Start()
    {
        PlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
        
        agent = GetComponent<NavMeshAgent>();
        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        OnDeath += HasDied;
        Defeat += RevertEnemy;
        dialog.Parent = this;
        Health = MaxHealth;
        TargetMarker = GameObject.Find("TargetMarker");
        NPCName.text = Name;
        
        if (HasWalkAnim)
            RoamingAnim = "Walking";
        else
            RoamingAnim = "Running";
        if (agent != null)
        {
            RunSpeed = agent.speed;
            WalkSpeed = RunSpeed / 2;
        }       
        if (IsEnemy)
            NPCName.color = Color.red;
        else
            NPCName.color = Color.green;
        
    }
    public virtual void TakeDamage(float Damage, Transform Sender, bool crit)
    {
        if(Target == null || Sender != Target.transform)
        {
            if(Target != null)
            NullEnemy();
            Target = Sender;
        }       
        if (Sender.gameObject.name == "Player" || Sender.gameObject.name == "Mouse")
        {
            PlayerLastHitter = true;
            DamageFromPlayer += Damage;
            HitEffect.Play();
            SetPopUpUi(Damage, Sender, crit);
        }
        else
            PlayerLastHitter = false;
        if(crit)
        Health -= Damage*2;
        else
            Health -= Damage;
        if(!Regen && !DontRegen)
        {
            StartCoroutine(HealthRegen());
            Regen = true;
        }        
    }
    protected void SetPopUpUi(float Value, Transform Sender, bool Crit)
    {
        GameObject t = ObjectPooler.Instance.SpawnFromPool("Text");
        t.transform.SetParent(Canvas.transform);
        t.transform.position = Canvas.transform.position;
        t.transform.rotation = Canvas.transform.rotation;
        //t.transform.localScale = NPCName.transform.localScale;
        
        if(Crit)
        {
            t.transform.localScale = new Vector2(7, 9);
            t.GetComponent<IText>().SetText(" Crit", Value * 2);
        }
        else
        {
            t.transform.localScale = new Vector2(3, 5);
            t.GetComponent<IText>().SetText(null, Value);
        }
        t.GetComponent<Text>().color = Color.red;
        t.SetActive(true);
    }
    public virtual void Clicked()
    {
        Player.CurrentTarget = this;
        //PlayerController.agent.stoppingDistance = PlayerStopDistance;
        TargetMarker.transform.position = gameObject.transform.position + new Vector3(0,0.1f,0);
        TargetMarker.transform.SetParent(gameObject.transform);
    }

    public virtual void PlayerStopped()
    {
        if (GetComponent<QuestGiver>())
        {
            GetComponent<QuestGiver>().PlayerStopped();
        }
        else
            if(!IsEnemy)
            DialogManager.Instance.UIText(dialog);
    }

    public virtual void HasDied(BaseNPC Sender)
    {
        NullEnemy();
        if(DamageFromPlayer >= MaxHealth / 2 || PlayerLastHitter)
        {
            Player.SetPopUpUi(ExpToGive, true);
            int RandomNum = Random.Range(0, 100); 
            if(RandomNum <= 30)
            {
                ItemManager.instance.maxValue = Level;
                Instantiate(ItemManager.instance.RandomItem(), transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(Random.Range(-135, -45), Random.Range(-135, -45), Random.Range(-135, -45))));
            }            
        }
        if(ThisMobLoot)
        {
            int RandomNum = Random.Range(0, 100);
            if(RandomNum <= ThisMobLootChance)
            Instantiate(ThisMobLoot, transform.position + new Vector3(0,0.5f,0), Quaternion.Euler(new Vector3(Random.Range(-135, -45), Random.Range(-135, -45), Random.Range(-135, -45))));
        }
        Anim.SetBool("Running", false);
        Anim.SetBool("Attacking", false);
        Anim.SetBool("Dead", true);
        if(FreezeGO != null)
            Destroy(FreezeGO);
        RB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<BaseNPC>().enabled = false;
        NPCName.gameObject.SetActive(false);       
        StartCoroutine(HideTimer());
    }
    
    protected IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(6);
        RemoveEnemy();
    }
    protected virtual void RemoveEnemy()
    {
        Destroy(gameObject);
    }
    public virtual void EnemyDied(BaseNPC Sender)
    {
        //Sender.OnDeath -= EnemyDied;
        NullEnemy();
        if (Anim != null)
        {
            Anim.SetBool("Running", false);
            Anim.SetBool("Attacking", false);
        }
        //IsAtTarget = false;
    }
    private void EnableEnemyRun(BaseNPC Sender)
    {
        IsAtTarget = false;
    }
    public void NullEnemy()
    {
        if(Target != null)
        {
            if (Target.gameObject.name != "Player")
                Target.GetComponent<BaseNPC>().OnDeath -= EnemyDied;
            else if (Target.gameObject.name == "Player")
            {
                Player.PlayerDied -= EnemyDied;
                Player.PlayerDied -= EnableEnemyRun;
            }
            Target = null;
        }       
    }
    
    public virtual void Freeze(float Time)
    {        
        IsFrozen = true;
        agent.isStopped = true;
        Target = null;
        Anim.SetBool("Running", false);
        FreezeGO = Instantiate(FreezeParticle, transform.position + new Vector3(0,2,0), transform.rotation);
        FreezeGO.transform.parent = transform;
        StartCoroutine(FreezeTimer(Time));
    }
    private IEnumerator FreezeTimer(float Time)
    {
        yield return new WaitForSeconds(Time);
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            IsFrozen = false;
            Destroy(FreezeGO);
        }         
    }

    public void TurnEnemy()
    {
        IsEnemy = true;
        TurnedEnemy = true;
        Target = Player.transform;
        NPCName.color = Color.red;
    }
    float Wait = 0.2f;
    float PushDelay = 0.8f;
    bool StartTimer;  
    private void Update()
    {
        if(StartTimer)
        {
            Wait -= Time.deltaTime;
            if (Wait <= 0)
            {
                ReturnToPosition = true;
                Wait = 0.2f;
                StartTimer = false;
            }
        }
        if(PushBack)
        {
            transform.position += direction * 15 * Time.deltaTime;
            PushDelay -= Time.deltaTime;
            if (PushDelay <= 0)
            {
                PushBack = false;
                RB.isKinematic = true;
            }
        }
    }
    Vector3 direction;
    public void StartPushBack(float value)
    {
        PushDelay = value;
        Vector3 moveDirection = transform.position - Player.transform.position;
        float distance = moveDirection.magnitude;
        direction = moveDirection / distance;
        PushBack = true;
        RB.isKinematic = false;
    }
    /*
    public IEnumerator PushBackEnd()
    {
        moveDirection = transform.position - Player.transform.position;
        PushBack = true;
        RB.isKinematic = false;
        yield return new WaitForSeconds(1);
        PushBack = false;
        RB.isKinematic = true;
    }
    */
    public void RevertEnemy()
    {
        IsEnemy = false;
        TurnedEnemy = false;
        Health = MaxHealth;
        agent.SetDestination(StartPos);
        StartTimer = true;
        if(!Player.IsDead)
        NullEnemy();
        NPCName.color = Color.green;
    }
}
interface IStopped
{
    void PlayerStopped();
}
public interface IFreezable
{
    void Freeze(float Time);
}

