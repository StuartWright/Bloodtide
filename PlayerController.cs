using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityStandardAssets.SceneUtils;
public class PlayerController : MonoBehaviour
{
    private Rigidbody RB;
    public Animator Anim;
    private Transform m_Cam;
    private BasePlayer Player;
    

    public delegate void RunToPoint();
    public event RunToPoint HidePoint;
    public delegate void PlayerStop(Transform transform);
    public event PlayerStop PlayerStopped;

    public GameObject InventoryPanel, SkillPanel;
    //public PlaceTargetWithMouse TargetPicker;
    public bool InventoryOpen, SkillPanelOpen;
    
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }
    private bool AutoRun;
    public Transform Target;
    public bool LookAtEnemy;
    public string AnimState;
    [SerializeField]
    private EquippedWeapon weapon;
    public EquippedWeapon Weapon
    {
        get { return weapon; }
        set
        {
            weapon = value;
            Anim.SetBool(AnimState, false);
            if (Weapon == EquippedWeapon.Melee || Weapon == EquippedWeapon.None)
            {
                AnimState = "Attacking";
            }
            else if(Weapon == EquippedWeapon.Bow)
            {
                AnimState = "BowAttack";
            }
            Player.SetSpellUI(Weapon);
            if(Player.CurrentTarget)
            {                
                GetDistance();
                if(Player.CurrentTarget.IsEnemy)
                SetTarget(Player.CurrentTarget.transform);
            }
        }
    }
    void Start()
    {
        Player = GetComponent<BasePlayer>();
        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        m_Cam = Camera.main.transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartCoroutine(Wait());
        SkillPanel.SetActive(false);
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        InventoryPanel.SetActive(false);
        ShopInventory.Instance.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (Target != null && AutoRun)
        {
            agent.SetDestination(Target.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                agent.isStopped = false;
                Anim.SetBool("Running", true);
                Anim.SetBool(AnimState, false);
                LookAtEnemy = false;
            }
            else
            {            
                if (Target.GetComponent<IStopped>() != null)
                {   
                    PlayerStopped(Target);
                }
                StopRunning();
            }
        }
        if (Player.CurrentTarget != null && LookAtEnemy)
        {
            if (Weapon == EquippedWeapon.Melee || Weapon == EquippedWeapon.None)
                transform.LookAt(Player.CurrentTarget.transform);
            else if (Weapon == EquippedWeapon.Bow)
            {
                transform.LookAt(Player.CurrentTarget.transform);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + 90, 0);
            }
        }
    }
    public float GetDistance()
    {
        if (Weapon == EquippedWeapon.Melee || Weapon == EquippedWeapon.None)
        {
            return agent.stoppingDistance = 2;
        }
        else if (Weapon == EquippedWeapon.Bow)
        {
            return agent.stoppingDistance = 15;
        }
        return 0;
    }
    public void OpenInventory()
    {
        if (!InventoryOpen)
        {
            InventoryPanel.SetActive(true);
            SkillPanel.SetActive(false);
            InventoryOpen = true;
            SkillPanelOpen = false;
        }
        else
        {
            InventoryPanel.SetActive(false);
            InventoryOpen = false;
        }
    }
    public void OpenSkillPanel()
    {
        if (!SkillPanelOpen)
        {
            SkillPanel.SetActive(true);
            InventoryPanel.SetActive(false);
            SkillPanelOpen = true;
            InventoryOpen = false;
        }
        else
        {
            SkillPanel.SetActive(false);
            SkillPanelOpen = false;
        }
    }
    public void StopRunning()
    {
        Target = null;
        agent.isStopped = true;
        Anim.SetBool("Running", false);
        AutoRun = false;
        HidePoint();
    }
    public void SetTarget(Transform target)
    {
        Target = target;
        agent.SetDestination(Target.position);
        
        StartCoroutine(AutoRunDelay());
    }
    
    public IEnumerator AutoRunDelay()
    {
        yield return new WaitForSeconds(0.1f);
        AutoRun = true;
    }
    
}
