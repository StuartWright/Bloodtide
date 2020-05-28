using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    private BasePlayer Player;
    private PlayerController PlayerController;
    public Text SkillPointsText;
    private int skillPoints;
    private Transform button;
    public int SkillPoints
    {
        get { return skillPoints; }
        set
        {
            skillPoints = value;
            SkillPointsText.text = "Skill Points: " + SkillPoints;
        }

    }

    void Start()
    {
        Instance = this;
        Player = GameObject.Find("Player").GetComponent<BasePlayer>();
        PlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
        SkillPoints = 10;
        /*
        for(int i = 0; i < Player.skills.Length; i++)
        {
            if (Player.skills[i].DescriptionText != null)
                Player.skills[i].Description();
        }
        */
    }

    public void UpgradeStrength()
    {
        if(SkillPoints > 0)
        {
            Player.Strength++;
            SkillPoints--;
        }
    }
    public void UpgradeAgility()
    {
        if (SkillPoints > 0)
        {
            Player.Agility++;
            SkillPoints--;
        }
    }
    public void UpgradeDexterity()
    {
        if (SkillPoints > 0)
        {
            Player.Dexterity++;
            SkillPoints--;
        }
    }
    public void UpgradeIntelligence()
    {
        if (SkillPoints > 0)
        {
            Player.Intelligence++;
            SkillPoints--;
        }
    }
    public void CheckSpell(int Index)
    {
        if (!Player.skills[Index].Unlocked)
        {
            if (SkillPoints >= Player.skills[Index].SkillPointsToUpgrade && Player.PlayerLevel >= Player.skills[Index].UnlockLevelRequired)
            {
                Player.skills[Index].Unlocked = true;
                SkillPoints -= Player.skills[Index].SkillPointsToUpgrade;
                Player.skills[Index].Description();
                button = EventSystem.current.currentSelectedGameObject.transform;
                button.GetComponentInChildren<Text>().text = "Upgrade: " + Player.skills[Index].SkillPointsToUpgrade + " Skill Points";
                Player.SetSpellUI(PlayerController.Weapon);
            }
        }
        else
            UpgradeSkill(Index);
    }
    public void UpgradeSkill(int Index)
    {
        if(SkillPoints >= Player.skills[Index].SkillPointsToUpgrade)
        {
            SkillPoints -= Player.skills[Index].SkillPointsToUpgrade;
            Player.skills[Index].SkillPointsToUpgrade++;
            Player.skills[Index].Unlocked = true;
            Player.skills[Index].SpellDamage += 5;
            Player.skills[Index].SpellManaCost += Player.skills[Index].SpellManaToAdd;
            Player.skills[Index].Value += Player.skills[Index].ValueToAdd;
            Player.skills[Index].Description();
            button = EventSystem.current.currentSelectedGameObject.transform;
            button.GetComponentInChildren<Text>().text = "Upgrade: " + Player.skills[Index].SkillPointsToUpgrade + " Skill Points";
        }        
    }

}
