using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    private Text TimerText;
    private PlayerController PlayerController;
    private BasePlayer Player;
    public bool IsCoolingDown;
    public int one, two;
    private float Timer1, Timer2;
    void Start()
    {
        PlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
        Player = GameObject.Find("Player").GetComponent<BasePlayer>();
        TimerText = transform.Find("Text").GetComponent<Text>();
    }

    
    void Update()
    {
        if(Timer1 > 0)
        {
            Timer1 -= Time.deltaTime;
            if (PlayerController.Weapon == EquippedWeapon.Melee)
            {               
                TimerText.text = "" + (int)Timer1;                
            }
            else
                TimerText.text = "";
            if (Timer1 <= 0)
            {
                //IsCoolingDown = false;
                TimerText.text = "";
            }
        }
        else if (Timer2 > 0)
        {
            Timer2 -= Time.deltaTime;
            if (PlayerController.Weapon == EquippedWeapon.Bow)
            {
                TimerText.text = "" + (int)Timer2;
            }
            else
                TimerText.text = "";
            if (Timer2 <= 0)
            {
                //IsCoolingDown = false;
                TimerText.text = "";
            }
        }
    }
    public void StartCoolDown(float Value)
    {
        if (PlayerController.Weapon == EquippedWeapon.Melee)
        {
            Timer1 = Value;
        }
        else if (PlayerController.Weapon == EquippedWeapon.Bow)
        {
            Timer2 = Value;
        }
        //IsCoolingDown = true;
    }
}
