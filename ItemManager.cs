using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public GameObject[] Items;
    private GameObject Loot;
    private BasePlayer PlayerRef;
    private int Strength;
    private int Intelligance;
    private int Dexterity;
    private int Agility;
    private int Defence;
    private int ItemPrice;
    public int WeapPhysicalDamage;
    //public int WeapMagicDamage;
    private int RandomItemStats;
    public int minValue, maxValue;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        PlayerRef = GameObject.Find("Player").GetComponent<BasePlayer>();
        foreach (GameObject T in Items)
        {
            T.GetComponent<ItemPickup>().item.StackAmount = 1;
        }
    }


    public GameObject RandomItem()
    {
        int RandomNum = Random.Range(0, Items.Length);
        Loot = Items[RandomNum];

        return Loot;
    }
    public void DropItem(Items item)
    {
        foreach (GameObject T in Items)
        {
            if (item.EquipmentType == T.GetComponent<ItemPickup>().item.EquipmentType)
            {
                Vector3 Pos;
                if(item.EquipmentType == EquipmentType.Constmable)
                    Pos = new Vector3(Random.Range(-2f, 2f), 0.3f, Random.Range(-2f, 2f));
                else
                Pos = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                GameObject DroppedItem = Instantiate(T, PlayerRef.gameObject.transform.position + Pos, Quaternion.Euler(new Vector3(-90, 10, 0)));
                item.fixedStats = true;
                DroppedItem.GetComponent<ItemPickup>().item = item;
                break;
            }
        }
    }
    private void GenerateStats(int MinValue, int MaxValue)
    {

        int RandomNum = Random.Range(0, 100);
        if (RandomNum <= 10)
        {
            // MaxValue = 4;
        }
        Strength = Random.Range(MinValue, MaxValue);
        Intelligance = Random.Range(MinValue, MaxValue);
        Dexterity = Random.Range(MinValue, MaxValue);
        Agility = Random.Range(MinValue, MaxValue);
        WeapPhysicalDamage = Random.Range(MinValue, MaxValue);
        Defence = Random.Range(MinValue, MaxValue);
        ItemPrice = (Strength *2) + Intelligance + Dexterity + Agility + (WeapPhysicalDamage * 2) + (Defence * 2) * 2;
        if (Strength <= 0)
            Strength = 0;
        if (Intelligance <= 0)
            Intelligance = 0;
        if (Dexterity <= 0)
            Dexterity = 0;
        if (Agility <= 0)
            Agility = 0;
        if (WeapPhysicalDamage <= 0)
            WeapPhysicalDamage = 1;
        if (Defence <= 0)
            Defence = 0;
        if (ItemPrice <= 0)
            ItemPrice = 1;
    }
    public Items createItem(Items pickup)
    {
        Items newItem = Instantiate(pickup);

        if (pickup.fixedStats == false)
        {
            switch (newItem.EquipmentType)
            {
                case EquipmentType.Weapon:
                    GenerateStats(minValue, maxValue);
                    newItem.Strength = Strength;
                    newItem.Intelligence = Intelligance;
                    newItem.Dexterity = Dexterity;
                    newItem.Agility = Agility;
                    newItem.PhysicalDamage = WeapPhysicalDamage;
                    newItem.ItemPrice = ItemPrice;
                    //newItem.MagicDamage = WeapMagicDamage;
                    break;
                case EquipmentType.Chest:
                    GenerateStats(minValue, maxValue);
                    newItem.Strength = Strength;
                    newItem.Intelligence = Intelligance;
                    newItem.Dexterity = Dexterity;
                    newItem.Agility = Agility;
                    newItem.Defence = Defence;
                    newItem.ItemPrice = ItemPrice;
                    break;
                case EquipmentType.Hands:
                    GenerateStats(minValue, maxValue);
                    newItem.Strength = Strength;
                    newItem.Intelligence = Intelligance;
                    newItem.Dexterity = Dexterity;
                    newItem.Agility = Agility;
                    newItem.Defence = Defence;
                    newItem.ItemPrice = ItemPrice;
                    break;
                case EquipmentType.Boots:
                    GenerateStats(minValue, maxValue);
                    newItem.Strength = Strength;
                    newItem.Intelligence = Intelligance;
                    newItem.Dexterity = Dexterity;
                    newItem.Agility = Agility;
                    newItem.Defence = Defence;
                    newItem.ItemPrice = ItemPrice;
                    break;
                case EquipmentType.Helmet:
                    GenerateStats(minValue, maxValue);
                    newItem.Strength = Strength;
                    newItem.Intelligence = Intelligance;
                    newItem.Dexterity = Dexterity;
                    newItem.Agility = Agility;
                    newItem.Defence = Defence;
                    newItem.ItemPrice = ItemPrice;
                    break;
            }
        }

        return newItem;
    }

}