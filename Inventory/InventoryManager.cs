
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;
    public GameObject SwordRef;
    public GameObject MagicWeapRef;
    public GameObject BowRef;
    private PlayerController Player;
    //private SkillManager SM;
    //private UIManager UI;
    private int StrengthToAdd;
    private int IntelligenceToAdd;
    private int StaminaToAdd;
    private void Awake()
    {
        inventory.OnItemRightClickEvent += EquipFromInventory;
        equipmentPanel.OnItemRightClickEvent += UnEquipFromInventory;
    }

    private void Start()
    {
        //SM = SkillManager.Instance;
        //UI = UIManager.instance;
        //UI.UpdateStats();
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void EquipFromInventory(Items item)
    {
        if(item is EquippableItems)
        {
            Equip((EquippableItems)item);
        }
    }
    
    private void UnEquipFromInventory(Items item)
    {
        if (item is EquippableItems)
        {
            UnEquip((EquippableItems)item);
        }
    }
    
    public void Equip(EquippableItems item)
    {
        if(inventory.RemoveItem(item))
        {
            EquippableItems PreviousItem;
            
            if (equipmentPanel.AddItem(item, out PreviousItem))
            {
                if (PreviousItem != null)
                {
                    /*
                    SM.Strength -= PreviousItem.Strength;
                    SM.Intelligance -= PreviousItem.Intelligance;
                    SM.Stamina -= PreviousItem.Stamina;
                    SM.Agility -= PreviousItem.Agility;
                    SM.WeapPhysicalDamage -= PreviousItem.PhysicalDamage;
                    SM.WeapMagicDamage -= PreviousItem.MagicDamage;
                    playerRef.MaxHealth -= StrengthToAdd;
                    playerRef.MaxMana -= IntelligenceToAdd;
                    playerRef.MaxStamina -= StaminaToAdd;
                    */
                    UnEquip(PreviousItem);
                    //item.Equipped = false;
                    //inventory.AddItem(PreviousItem);
                }
                item.ApplyStats();
                /*
                SM.Strength += item.Strength;
                SM.Intelligance += item.Intelligance;
                SM.Stamina += item.Stamina;
                SM.Agility += item.Agility;
                SM.WeapPhysicalDamage += item.PhysicalDamage;
                SM.WeapMagicDamage += item.MagicDamage;
                StrengthToAdd = item.Strength * 10;
                playerRef.MaxHealth += StrengthToAdd;
                IntelligenceToAdd = item.Intelligance * 10;
                playerRef.MaxMana += IntelligenceToAdd;
                StaminaToAdd = item.Stamina * 10;
                playerRef.MaxStamina += StaminaToAdd;
                */
                item.Equipped = true;
                
                if (item.weaponType == WeaponType.Sword)
                {
                    SwordRef.SetActive(true);
                    Player.Weapon = EquippedWeapon.Melee;
                    //SM.MagicWeaponEquipped = false;
                    //SM.PhysicalWeaponEquipped = true;
                    //UI.CheckIcons();
                    //MagicWeapRef.SetActive(false);
                }
                else if(item.weaponType == WeaponType.Bow)
                {
                    BowRef.SetActive(true);
                    Player.Weapon = EquippedWeapon.Bow;
                    //SM.MagicWeaponEquipped = true;
                    //SM.PhysicalWeaponEquipped = false;
                    //UI.CheckIcons();
                    //SwordRef.SetActive(false);
                }
                
            }
            else
            {               
                inventory.AddItem(item);
                return;
            }
            inventory.RefreshUI();
        }
    }

    public void UnEquip(EquippableItems item)
    {
        if (!inventory.IsFull())// && equipmentPanel.RemoveItem(item))
        {
            Player.Weapon = EquippedWeapon.None;
            item.RemoveStats();
            /*
            SM.Strength -= item.Strength;
            SM.Intelligance -= item.Intelligance;
            SM.Stamina -= item.Stamina;
            SM.Agility -= item.Agility;
            SM.WeapPhysicalDamage -= item.PhysicalDamage;
            SM.WeapMagicDamage -= item.MagicDamage;
            playerRef.MaxHealth -= StrengthToAdd;
            playerRef.MaxMana -= IntelligenceToAdd;
            playerRef.MaxStamina -= StaminaToAdd;
            */
            item.Equipped = false;
            
            equipmentPanel.RemoveItem(item);
            inventory.AddItem(item);

            if (item.weaponType == WeaponType.Sword)
            {
                SwordRef.SetActive(false);
                //SM.PhysicalWeaponEquipped = false;
            }
            else if (item.weaponType == WeaponType.Bow)
            {
                BowRef.SetActive(false);
                //MagicWeapRef.SetActive(false);
                //SM.MagicWeaponEquipped = false;
            }
           
            //inventory.RefreshUI();///////////////////////////
        }
    }


}
