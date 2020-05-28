using UnityEngine.UI;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Skills
{
    //public delegate void Cooldown();
    //public event Cooldown Skill1Cooldown;
    public float SpellManaCost;
    public float SpellManaToAdd;
    public float Value;
    public float ValueToAdd;
    public float SpellDamage;
    public float EnemyStoppingDistance, CoolDownTime;
    public int SkillPointsToUpgrade, UnlockLevelRequired;
    public bool CanAlwaysCast, Unlocked, CoolDown;
    public string DescriptionPart1;
    public string DescriptionPart2;
    public Text DescriptionText, ManaCostText;
    public SkillCooldown Cooldowns;
    public BasePlayer Player;
    public void Description()
    {
        DescriptionText.text = DescriptionPart1 + " " + ("<color=#" + ColorUtility.ToHtmlStringRGBA(Color.red) + ">" + SpellDamage + "</color>") + " " + DescriptionPart2;
        //ManaCostText.text = "Requires: " + SpellManaCost + " mana";
        ManaCostText.text = "Requires: " + ("<color=#" + ColorUtility.ToHtmlStringRGBA(Color.blue) + ">" + SpellManaCost + "</color>") + " mana";
    }  
    public IEnumerator CoolDownTimer()
    {
        CoolDown = true;
        Player.SpellUIState();
        Cooldowns.StartCoolDown(CoolDownTime);        
        yield return new WaitForSeconds(CoolDownTime);
        CoolDown = false;
        Player.SpellUIState();
    }
}

  
