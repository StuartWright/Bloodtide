using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.SceneUtils;
public enum EquippedWeapon
{ 
    Melee,
    Bow,
    None
}


public class BasePlayer : MonoBehaviour, IDamagable, IPickup
{
    public delegate void Death(BaseNPC Sender);//Not right, dont want it to be baseNpc but quick fix.
    public event Death PlayerDied;
    public delegate void LevelUpEvent();
    public event LevelUpEvent LeveledUp;
    public Skills[] skills = new Skills[3];
    public Text HealthText, ManaText, ExpText, PlayerLevelText;
    public Text EnemyHealthText;
    public Text EnemyNameText;
    public Text DamageText, StrengthText, AgilityText, DexterityText, IntelligenceText, FireRateText, DefenceText, CritChanceText, MoneyText;
    private int SkillIndex;
    public int PlayerLevel;
    public bool IsCasting;
    public bool IsDead;
    public Inventory inventory;
    public GameObject Canvas, QuestTextParent;
    public List<Text> QuestTitles = new List<Text>();
    public List<Text> QuestDesctiptions = new List<Text>();
    public ParticleSystem AoeAttackTrail;
    public ParticleSystem AoeAttackCircle, LevelUpEffect;
    public GameObject[] AoeCircleParts;
    public Slider HPBar, MPBar, ExpBar, EnemyBar;
    public Button[] SkillButtons;
    public Button AttackButtons;
    public Image[] SkillButtonsImages;
    public Sprite[] SkillsImgs;
    public Sprite UnavaliableSkill;
    public float MaxHealth, BaseHealth, MaxMana, BaseMana, ExpToReach;
    public float Damage;
    public float AttackRate, CritChance;
    public Vector3 SpawnPoint;
    public PlayerController Controller;
    public GameObject EnemyhpPanel, LevelUpText, DiedPanel;
    public Rigidbody RB;
    public Transform BowShootPoint;
    public GameObject Arrow, SkillArrow, SkillArrow2, MeleeSkill1, Pet, HomingArrow;
    public Animation ManaAnim;
    public Animator BlackFade;
    public FreeLookCam FreeCam;

    [SerializeField]
    private int money;
    public int Money
    {
        get { return money; }
        set
        {
            money = value;
            //MoneyText.text = "Money: " + Money;
        }
    }

    private float physicalDamage;
    public float PhysicalDamage
    {
        get { return physicalDamage; }
        set
        {
            physicalDamage = value;
            CalculateDamage();
        }
    }
    private int defence;
    public int Defence
    {
        get { return defence; }
        set
        {
            defence = value;
            DefenceText.text = "Defence: " + Defence;
        }
    }

    private float strength;
    public float Strength
    {
        get { return strength; }
        set
        {
            strength = value;
            StrengthText.text = "Strength: " + Strength;
            MaxHealth = BaseHealth + (Strength * 10);            
            CalculateDamage();
        }
    }
    private float agility;
    public float Agility
    {
        get { return agility; }
        set
        {
            agility = value;
            AgilityText.text = "Agility: " + Agility;
            AttackRate = 1.5f;//base attackrate
            AttackRate += (Agility / 100) * AttackRate;
            FireRateText.text = "Attack Speed: " + AttackRate.ToString("F1");
        }
    }
    private float dexterity;
    public float Dexterity
    {
        get { return dexterity; }
        set
        {
            dexterity = value;
            DexterityText.text = "Dexterity: " + Dexterity;
            CritChance = Dexterity / 2;
            CritChanceText.text = "Crit chance: " + CritChance + "%";
        }
    }
    private float intelligence;
    public float Intelligence
    {
        get { return intelligence; }
        set
        {
            intelligence = value;
            IntelligenceText.text = "Intelligence: " + Intelligence;
            MaxMana = BaseMana + (Intelligence * 10);
        }
    }
    public void CalculateDamage()
    {
        float dam;
        dam = PhysicalDamage + (Strength / 50) * PhysicalDamage;// strength%(*2) of physicalDamage
        DamageText.text = "Damage: " + dam.ToString("F0");
        Damage = dam;
    }

    private float health;
    public float Health
    {
        get { return health; }
        set
        {
            health = Mathf.Max(Mathf.Min(value, MaxHealth), 0);
            if (Health <= 0)
            {
                PlayerDied(null);
            }
            HealthText.text = Health.ToString("F0") + "/" + MaxHealth;
            HPBar.value = Health / MaxHealth;
        }
    }
    private float mana;
    public float Mana
    {
        get { return mana; }
        set
        {
            mana = Mathf.Max(Mathf.Min(value, MaxMana), 0);
            ManaText.text = Mana.ToString("F0") + "/" + MaxMana;
            MPBar.value = Mana / MaxMana;
        }
    }
    private float exp;
    public float Exp
    {
        get { return exp; }
        set
        {
            exp = value;
            ExpText.text = Exp + "/" + ExpToReach + " Exp";
            ExpBar.value = Exp / ExpToReach;
            if(Exp >= ExpToReach)
            {
                LeveledUp?.Invoke();
                PlayerLevel++;
                PlayerLevelText.text = ""+PlayerLevel;
                BaseHealth += 10;
                BaseMana += 10;
                MaxHealth += 10;
                MaxMana += 10;
                Health = MaxHealth;
                Mana = MaxMana;
                LevelUpEffect.Play();
                SkillManager.Instance.SkillPoints++;
                LevelUpText.SetActive(true);
                if(Exp > ExpToReach)
                {
                    float RemainingExp = Exp;
                    RemainingExp -= ExpToReach;
                    ExpToReach += 10;
                    Exp = RemainingExp;
                }
                else
                {
                    ExpToReach += 10;
                    Exp = 0;                    
                }
            }
        }
    }
    public List<Quest> Quests = new List<Quest>();
    private BaseNPC currentTarget;
    public BaseNPC CurrentTarget
    {
        get { return currentTarget; }
        set
        {
            currentTarget = value;
            if(CurrentTarget != null)
            {
                EnemyhpPanel.SetActive(true);                
                EnemyNameText.text = "" + CurrentTarget.Name;
                if (CurrentTarget.IsEnemy)
                    EnemyNameText.color = Color.red;
                else
                    EnemyNameText.color = Color.green;
                EnemyHealth = CurrentTarget.Health;
                Controller.Anim.SetFloat("AttackSpeed", AttackRate);
                //FreeCam.ResetCam();
                AttackButtons.interactable = true;
            }
            else
            {
                EnemyhpPanel.SetActive(false);
                Controller.LookAtEnemy = false;
                AttackButtons.interactable = false;
            }
            SpellUIState();
        }
    }
    private float enemyHealth;
    public float EnemyHealth
    {
        get { return enemyHealth; }
        set
        {
            enemyHealth = value;
            EnemyHealthText.text = EnemyHealth.ToString("F0") + "/" + CurrentTarget.MaxHealth;
            EnemyBar.value = EnemyHealth / CurrentTarget.MaxHealth;
        }
    }

    void Start()
    {
        Controller = GetComponent<PlayerController>();
        Controller.PlayerStopped += WhosTheTarget;
        PlayerDied += HasDied;
        HealthText.text = "" + Health;
        RB = GetComponent<Rigidbody>();
        BaseMana = MaxMana;
        BaseHealth = MaxHealth;
        Health = MaxHealth;
        Mana = MaxMana;
        PhysicalDamage = 3;
        Exp = 0;
        Strength = 0;
        Agility = 0;
        Dexterity = 0;
        Intelligence = 0;
        Defence = 0;        
        SetSpellUI(Controller.Weapon);        
        StartCoroutine(Skill1Effect());
    }
    private void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            
        }       
    }
    public void PlayBlackFadeIn() 
    {        
        BlackFade.SetBool("FadeOut", false);
        BlackFade.SetBool("FadeIn", true);
    }
    public void PlayBlackFadeOut()
    {
        DialogManager.Instance.AcceptClicked -= PlayBlackFadeOut;
        BlackFade.SetBool("FadeIn", false);
        BlackFade.SetBool("FadeOut", true);
    }
    private void OnEnable()
    {
        StartCoroutine(RegainStats());
    }
    private IEnumerator RegainStats()
    {
        yield return new WaitForSeconds(1);
        if(!IsDead)
        {
            if (Health < MaxHealth)
                Health += Strength;
            if (Mana < MaxMana)
                Mana += Intelligence;
        }
        StartCoroutine(RegainStats());
    }
    
    public void UpdateKillQuest(BaseNPC Sender)
    {
        for(int i = 0; i < Quests.Count; i++)
        {
            if (Quests[i].Target != null && Sender.Name == Quests[i].Target.Name)
            {
                Quests[i].CurrentAmount++;
            }
        }             
    }
    public void UpdateList()
    {
        foreach(Quest quest in Quests)
        {
            quest.SetText();
        }
    }
    public void NullQuestTexts()
    {
        foreach (Quest quest in Quests)
        {
            quest.NullText();
        }
    }
    private void WhosTheTarget(Transform transform)
    {
        if (transform.GetComponent<BaseNPC>() && CurrentTarget!= null && CurrentTarget.IsEnemy)
        {
            Controller.LookAtEnemy = true;
            AttackEnemy();
        }            
        else
           transform.GetComponent<IStopped>().PlayerStopped();
           //CurrentTarget.GetComponent<IStopped>().PlayerStopped();
    }

    private void HasDied(BaseNPC Sender)//will change later
    {
        DiedPanel.SetActive(true);
        CurrentTarget = null;
        IsDead = true;
        PlaceTargetWithMouse.Instance.CanMove = false;
        Controller.Anim.SetBool("Running", false);
        Controller.Anim.SetBool(Controller.AnimState, false);
        Controller.Anim.SetBool("Dead", true);
        GetComponent<BoxCollider>().enabled = false;
    }

    public void Respawn()
    {
        DiedPanel.SetActive(false);
        gameObject.SetActive(false);
        gameObject.transform.position = SpawnPoint;
        gameObject.SetActive(true);
        Health = 10;
        Mana = 10;
        IsDead = false;
        PlaceTargetWithMouse.Instance.CanMove = true;
        Controller.Anim.SetBool("Dead", false);
        GetComponent<BoxCollider>().enabled = true;
    }
    public void TakeDamage(float Damage, Transform Sender, bool crit)
    {
        if(!CurrentTarget)
        {
            Sender.GetComponent<BaseNPC>().Clicked();
            Controller.GetDistance();
        }
        Damage = Damage -= Defence;
        if (Damage <= 0)
            Damage = 1;
        SetPopUpUi(Damage, false);
        Health -= Damage;
    }
    public void SetPopUpUi(float Value, bool IsExp)
    {
        GameObject t = ObjectPooler.Instance.SpawnFromPool("Text");
        t.transform.SetParent(Canvas.transform);
        t.transform.position = Canvas.transform.position;
        t.transform.rotation = Canvas.transform.rotation;
        t.transform.localScale = new Vector2(4,4);
        if(IsExp)
        {
            t.GetComponent<Text>().color = Color.green;
            t.GetComponent<IText>().SetText(" Exp",Value);
            Exp += Value;
        }
        else
        {
            t.GetComponent<Text>().color = Color.yellow;           
            t.GetComponent<IText>().SetText(null, Value);
        }

        t.SetActive(true);
    }
    private void AttackEnemy()
    {        
        if(!IsCasting)
        {
            Controller.Anim.SetBool(Controller.AnimState, true);
        }        
    }
    public void DealDamage()
    {
        if(CurrentTarget != null)
        {
            CurrentTarget.GetComponent<IDamagable>().TakeDamage(Damage, transform, IsCrit());      
        }       
    }
    public void FireArrow()
    {
        if(CurrentTarget)
        {
            Arrow arrow = Instantiate(Arrow, BowShootPoint.transform.position, transform.rotation).GetComponent<Arrow>();
            arrow.Target = CurrentTarget.transform;
            CurrentTarget.OnDeath += arrow.TargetDied;
            arrow.Sender = this.transform;
            arrow.Damage = Damage;
            if (IsCrit())
                arrow.Crit = true;
        }       
    }
    public void StopAnim() 
    {
        
        if (CurrentTarget != null && Controller.agent.remainingDistance <= Controller.agent.stoppingDistance)
        {
            AttackEnemy();
        }    
        else
            Controller.Anim.SetBool(Controller.AnimState, false);
    }
    public void GetTarget() 
    {
        if(CurrentTarget != null)
        {
            Controller.SetTarget(CurrentTarget.transform);
        }
    }
    public void SetSpellUI(EquippedWeapon weapon)
    {
        int i = 0;
        int j;
        switch (weapon)
        {
            case EquippedWeapon.Melee:
                j = 0;
                for(; i < SkillButtonsImages.Length; i++)
                {
                    if(skills[j].Unlocked)
                    SkillButtonsImages[i].sprite = SkillsImgs[j];
                    else
                        SkillButtonsImages[i].sprite = UnavaliableSkill;
                    j++;
                }                
                break;
            case EquippedWeapon.Bow:
                j = 4;
                for (; i < SkillButtonsImages.Length; i++)
                {
                    if (skills[j].Unlocked)
                        SkillButtonsImages[i].sprite = SkillsImgs[j];
                    else
                        SkillButtonsImages[i].sprite = UnavaliableSkill;
                    j++;
                }
                break;
            case EquippedWeapon.None:
                for (; i < SkillButtonsImages.Length; i++)
                {
                    SkillButtonsImages[i].sprite = UnavaliableSkill;
                }
                break;
        }
        SpellUIState();
    }
    public void SpellUIState()
    {
        int M = 0;
        int B = 4;
        for (int i = 0; i < SkillButtons.Length; i++)
        {
            if (Controller.Weapon == EquippedWeapon.Melee)
            {
                if(skills[M].CoolDown || !skills[M].Unlocked)
                {
                    SkillButtons[i].interactable = false;
                    break;
                }
                if (skills[M].CanAlwaysCast || CurrentTarget)                    
                    SkillButtons[i].interactable = true;
                else if (!CurrentTarget)
                    SkillButtons[i].interactable = false;
                
                M++;
            }
            else if (Controller.Weapon == EquippedWeapon.Bow)
            {
                if (skills[B].CoolDown)
                {
                    SkillButtons[i].interactable = false;
                    break;
                }
                if (!CurrentTarget || !skills[B].Unlocked)
                    SkillButtons[i].interactable = false;
                else if (skills[B].CanAlwaysCast || CurrentTarget)
                    SkillButtons[i].interactable = true;
                B++;
            }
            else if(Controller.Weapon == EquippedWeapon.None)
            {
                SkillButtons[i].interactable = false;
            }
        }
    }
    private bool IsCrit()
    {
        float RandomNum = Random.Range(0, 100);
        if (RandomNum <= CritChance)
            return true;
        else
            return false;
    }
    public void Spell1()
    {
        if (Controller.Weapon == EquippedWeapon.Melee)
        {
            if (CurrentTarget.IsEnemy && Mana >= skills[0].SpellManaCost)
            {
                PrepareSpell(0);
            }
            else
                ManaAnim.Play();
        }
        else if (Controller.Weapon == EquippedWeapon.Bow)
        {
            if (CurrentTarget.IsEnemy && Mana >= skills[4].SpellManaCost)
            {
                PrepareSpell(4);
            }
            else
                ManaAnim.Play();
        }
    }
    public void Spell2()
    {
        if (Controller.Weapon == EquippedWeapon.Melee)
        {
            if (CurrentTarget.IsEnemy && Mana >= skills[1].SpellManaCost)
            {
                PrepareSpell(1);
            }
            else
                ManaAnim.Play();
        }
        else if (Controller.Weapon == EquippedWeapon.Bow)
        {
            if (CurrentTarget.IsEnemy && Mana >= skills[5].SpellManaCost)
            {
                PrepareSpell(5);
            }
            else
                ManaAnim.Play();
        }
    }
    public void Spell3()
    {
        if (Controller.Weapon == EquippedWeapon.Melee)
        {
            if (Mana >= skills[2].SpellManaCost)
            {
                IsCasting = true;
                Controller.Anim.SetBool("Spell3", true);
            }
            else
                ManaAnim.Play();
        }
        else if (Controller.Weapon == EquippedWeapon.Bow)
        {
            if (CurrentTarget.IsEnemy && Mana >= skills[6].SpellManaCost)
            {
                PrepareSpell(6);
            }
            else
                ManaAnim.Play();
        }
    }
    public void PrepareSpell(int Index)
    {
        SkillIndex = Index;
        Controller.agent.isStopped = false;
        Controller.agent.stoppingDistance = skills[SkillIndex].EnemyStoppingDistance;
        //Controller.TargetPicker.RunningToNpc = true;
        Controller.SetTarget(CurrentTarget.transform);
        Controller.PlayerStopped += GetSpell;
    }
    private void GetSpell(Transform transform)
    {
        Controller.Anim.SetBool(Controller.AnimState, false);
        PlaceTargetWithMouse.Instance.CanMove = false;
        switch (SkillIndex)
        {         
            case 0:
                Controller.PlayerStopped -= GetSpell;
                IsCasting = true;
                Controller.Anim.SetBool("Spell1", true);
                    
                break;
            case 1:
                Controller.PlayerStopped -= GetSpell;
                IsCasting = true;
                Controller.Anim.SetBool("Spell2", true);
                break;
            case 2:
                //Controller.PlayerStopped -= GetSpell;
                
                break;
            case 3:
                
                break;
            case 4:
                Controller.PlayerStopped -= GetSpell;
                IsCasting = true;
                Controller.Anim.SetBool("BowSkill1", true);
                break;
            case 5:
                Controller.PlayerStopped -= GetSpell;
                IsCasting = true;
                Controller.Anim.SetBool("BowSkill2", true);
                break;
            case 6:
                Controller.PlayerStopped -= GetSpell;
                IsCasting = true;
                Controller.Anim.SetBool("BowSkill3", true);
                break;
        }       
    }
    public void ActivateSpell1()
    {
        if(CurrentTarget)
        {
            MeleeSkill1.SetActive(true);
            MeleeSkill1.transform.position = CurrentTarget.transform.position;
            MeleeSkill1.transform.parent = CurrentTarget.transform;
            //Rigidbody rb = CurrentTarget.GetComponent<Rigidbody>();
            //StartCoroutine(CurrentTarget.PushBackEnd());
            //rb.AddForce(gameObject.transform.forward * CurrentTarget.PushBackForce);
            CurrentTarget.TakeDamage(skills[SkillIndex].SpellDamage, transform, IsCrit());
            StartCoroutine(Skill1Effect());
            Mana -= skills[SkillIndex].SpellManaCost;
            StartCoroutine(skills[SkillIndex].CoolDownTimer());
        }        
    }
    private IEnumerator Skill1Effect()
    {
        yield return new WaitForSeconds(2);        
        MeleeSkill1.transform.parent = null;
        MeleeSkill1.SetActive(false);
    }
    public void EndSpell1()
    {        
        Controller.Anim.SetBool("Spell1", false);
        PlaceTargetWithMouse.Instance.CanMove = true;
        IsCasting = false;
        if (CurrentTarget != null)
        {
            Controller.SetTarget(CurrentTarget.transform);
        }
    }
   
    public void ActivateSpell2()
    {
        AoeAttackCircle.transform.position = CurrentTarget.transform.position;
        foreach(GameObject T in AoeCircleParts)
        {
            T.transform.localScale = new Vector3(skills[SkillIndex].Value / 3, skills[SkillIndex].Value / 3, 1);
        }
        AoeAttackCircle.Play();
        Collider[] Hit;
        Hit = Physics.OverlapSphere(CurrentTarget.transform.position, skills[SkillIndex].Value);
        foreach (Collider target in Hit)
        {
            if (target.tag == "NPC" && target.gameObject != gameObject && target.GetComponent<BaseNPC>().IsEnemy)
            {
                target.GetComponent<IDamagable>().TakeDamage(skills[SkillIndex].SpellDamage, transform, IsCrit());
            }
        }
        Mana -= skills[SkillIndex].SpellManaCost;
        StartCoroutine(skills[SkillIndex].CoolDownTimer());
    }   
    public void EndSpell2()
    {
        Controller.Anim.SetBool("Spell2", false);
        PlaceTargetWithMouse.Instance.CanMove = true;
        IsCasting = false;
        if (CurrentTarget != null)
        {
            //Controller.agent.stoppingDistance = CurrentTarget.PlayerStopDistance;
            Controller.GetDistance();
            Controller.SetTarget(CurrentTarget.transform);
        }
    }
    public void ActivateSpell3()
    {
       Pet pet = Instantiate(Pet, transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)), transform.rotation).GetComponent<Pet>();
        pet.PlayerTarget = transform;
        pet.Damage = (int)skills[2].Value;
        pet.Health = skills[2].Value *2;
        Mana -= skills[2].SpellManaCost;
        StartCoroutine(skills[2].CoolDownTimer());
    }
    public void EndSpell3()
    {
        Controller.Anim.SetBool("Spell3", false);
        PlaceTargetWithMouse.Instance.CanMove = true;
        IsCasting = false;
    }
    public void ActivateBowSpell1()
    {
        if(CurrentTarget)
        {
            Arrow arrow = Instantiate(SkillArrow, BowShootPoint.transform.position, transform.rotation).GetComponent<Arrow>();
            arrow.Target = CurrentTarget.transform;
            CurrentTarget.OnDeath += arrow.TargetDied;
            arrow.Sender = this.transform;
            arrow.Damage = skills[SkillIndex].SpellDamage;
            arrow.Value = skills[SkillIndex].Value;
            if (IsCrit())
                arrow.Crit = true;
            Mana -= skills[SkillIndex].SpellManaCost;
            StartCoroutine(skills[SkillIndex].CoolDownTimer());
        }       
    }
    public void EndBowSpell1()
    {
        Controller.Anim.SetBool("BowSkill1", false);
        PlaceTargetWithMouse.Instance.CanMove = true;
        IsCasting = false;
        if (CurrentTarget != null)
        {
            Controller.SetTarget(CurrentTarget.transform);
        }
    }
    public void ActivateBowSpell2()
    {
        if (CurrentTarget)
        {
            Arrow arrow = Instantiate(SkillArrow2, BowShootPoint.transform.position, transform.rotation).GetComponent<Arrow>();
            arrow.Target = CurrentTarget.transform;
            CurrentTarget.OnDeath += arrow.TargetDied;
            arrow.Sender = this.transform;
            arrow.Freeze = true;
            arrow.Value = skills[SkillIndex].Value;
            arrow.Damage = skills[SkillIndex].SpellDamage;
            if (IsCrit())
                arrow.Crit = true;
            Mana -= skills[SkillIndex].SpellManaCost;
            StartCoroutine(skills[SkillIndex].CoolDownTimer());
        }
    }
    public void EndBowSpell2()
    {
        Controller.Anim.SetBool("BowSkill2", false);
        PlaceTargetWithMouse.Instance.CanMove = true;
        IsCasting = false;
        if (CurrentTarget != null)
        {
            Controller.SetTarget(CurrentTarget.transform);
        }
    }
    public void ActivateBowSpell3()
    {
        if (CurrentTarget)
        {
            Vector3 Center = transform.position + new Vector3(0,2,0);
            for(int i = 0; i < skills[SkillIndex].Value; i++)
            {
                float a = i * (360 / skills[SkillIndex].Value);
                Vector3 pos = RandomCircle(Center, 0.1f, a);
                Quaternion rot = Quaternion.FromToRotation(Vector3.back, Center - pos);
                HomingArrow arrow = Instantiate(HomingArrow, pos, rot).GetComponent<HomingArrow>();
                arrow.Sender = transform;
                arrow.Damage = skills[SkillIndex].SpellDamage;
                arrow.Crit = IsCrit();
            }
            
            Mana -= skills[SkillIndex].SpellManaCost;
            StartCoroutine(skills[SkillIndex].CoolDownTimer());
        }
    }
    public void EndBowSpell3()
    {
        Controller.Anim.SetBool("BowSkill3", false);
        PlaceTargetWithMouse.Instance.CanMove = true;
        IsCasting = false;
        if (CurrentTarget != null)
        {
            Controller.SetTarget(CurrentTarget.transform);
        }
    }
    private Vector3 RandomCircle(Vector3 center, float radius, float a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
    public void AoeTrail()
    {
        if (CurrentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, CurrentTarget.transform.position);
            AoeAttackTrail.gameObject.SetActive(true);
            AoeAttackTrail.Play();
            StartCoroutine(StopTrail(distance / 10));
        }
        else
            return;
        
    }
    IEnumerator StopTrail(float Amount)
    {
        Controller.LookAtEnemy = true;
        yield return new WaitForSeconds(Amount);
        Controller.LookAtEnemy = false;
        ActivateSpell2();
        AoeAttackTrail.gameObject.SetActive(false);
    }
    
    public void PickUp(ItemPickup item)
    {
        //ItemPickup pickup = item.GetComponent<ItemPickup>();

        
        if(!inventory.IsFull())
        {
            Items newItem = ItemManager.instance.createItem(item.item);
            newItem.player = this;
            inventory.AddItem(newItem);
            if (Quests.Count > 0)
            {
                for (int i = 0; i < Quests.Count; i++)
                {
                    if (Quests[i].Type == QuestType.Collect && newItem.ItemName == Quests[i].CollectionItem.ItemName)
                    {
                        Quests[i].CurrentAmount++;
                    }
                }
            }
            Destroy(item.gameObject);
        }       
    }
}
public interface IPickup
{
    void PickUp(ItemPickup item);
}