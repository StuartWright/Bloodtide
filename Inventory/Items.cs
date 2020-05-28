using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
 HealthPotion,
 ManaPotioin,
 Miscellaneous,
 Equipment
 
}

public enum ShopType
{
    Weapon,
    Armor,
    Misc,
    UnSellable
}
[CreateAssetMenu]
public class Items : ScriptableObject
{
    public bool CanStack;
    public bool Equipped = false, fixedStats;

    public BasePlayer player;
    public Sprite ItemIcon;
    public ItemType type;
    public EquipmentType EquipmentType;
    public ShopType ShopItem;

    public int AmountToAdd;
    public int Strength;
    public int Intelligence;
    public int Dexterity;
    public int Agility;
    public int PhysicalDamage;
    public int Defence;
    //[Range(0, 999)]
    public int StackAmount = 1;
    public int ItemPrice;

    public string ItemName;
    public string ItemDescription;


    public void Use(Items item)
    {
        switch (type)
        {
            case ItemType.HealthPotion:
                    player.Health += AmountToAdd;
                break;
            case ItemType.ManaPotioin:
                    player.Mana += AmountToAdd;
                break;
            default:
                
                break;
        }
    }

    public void ApplyStats()
    {
        player.PhysicalDamage += PhysicalDamage;
        player.Defence += Defence;
        player.Strength += Strength;
        player.Intelligence += Intelligence;
        player.Dexterity += Dexterity;
        player.Agility += Agility;
    }
    public void RemoveStats()
    {
        player.PhysicalDamage -= PhysicalDamage;
        player.Defence -= Defence;
        player.Strength -= Strength;
        player.Intelligence -= Intelligence;
        player.Dexterity -= Dexterity;
        player.Agility -= Agility;
    }
}
