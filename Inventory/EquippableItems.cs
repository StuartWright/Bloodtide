using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Helmet,
    Chest,
    Boots,
    Hands,
    Constmable
}

public enum WeaponType
{
    Sword,
    MagicWeap,
    Bow,
    NotWeapon
}
    

[CreateAssetMenu]
public class EquippableItems : Items
{
    
    public WeaponType weaponType;
    

}


