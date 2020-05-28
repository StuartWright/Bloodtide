using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;

public class ShopItemSlot : MonoBehaviour, IPointerClickHandler//, IPointerDownHandler
{
    public event Action<Items> OnRightClickEvent;

    public Image Image;
    [SerializeField] Items item;
    public ShopInventory inventory;
    private BasePlayer PlayerRef;
    [SerializeField] Text AmountText;
    public bool IsEquipmentSlot;
    public float Timer;
    public bool PlayerSlot;
    private bool ItemDropped;

    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemStatText;
    [SerializeField] Text ItemDescriptionText;
    [SerializeField] Text ItemPrice;
    private StringBuilder sb = new StringBuilder();
    public EquipmentPanel EP;
    public Items Item
    {
        get
        { return item; }

        set
        {
            item = value;

            if (item == null)
            {
                Image.enabled = false;
            }
            else
            {
                Image.sprite = item.ItemIcon;
                Image.enabled = true;
                if (!GetComponent<EquipmentSlots>())
                    ShowToolTip(item);
            }

        }
    }
    
    public void ShowToolTip(Items item)
    {
        ItemNameText.text = item.ItemName;
        //// ItemSlotText.text = item.EquipmentType.ToString();
        ItemDescriptionText.text = "" + item.ItemDescription;
        sb.Length = 0;
        EquipmentType type;
        type = item.EquipmentType;
        int Index;
        switch (type)
        {
            case EquipmentType.Weapon:
                Index = 0;
                break;
            case EquipmentType.Chest:
                Index = 1;
                break;
            case EquipmentType.Helmet:
                Index = 2;
                break;
            case EquipmentType.Hands:
                Index = 3;
                break;
            case EquipmentType.Boots:
                Index = 4;
                break;
            case EquipmentType.Constmable:
                Index = 5;
                break;
            default:
                Index = 9;////////////////
                break;
        }
        Color colour = Color.black;
        if (Index != 5)
        {            
            if (EP.EquipmentSlots[Index].item == null || item.Strength == EP.EquipmentSlots[Index].item.Strength)
                colour = Color.black;
            else if (item.Strength < EP.EquipmentSlots[Index].item.Strength)
                colour = Color.red;
            else if (item.Strength > EP.EquipmentSlots[Index].item.Strength)
                colour = Color.green;
            AddStat(item.Strength, "Strength", colour);

            if (EP.EquipmentSlots[Index].item == null || item.Intelligence == EP.EquipmentSlots[Index].item.Intelligence)
                colour = Color.black;
            else if (item.Intelligence < EP.EquipmentSlots[Index].item.Intelligence)
                colour = Color.red;
            else if (item.Intelligence > EP.EquipmentSlots[Index].item.Intelligence)
                colour = Color.green;
            AddStat(item.Intelligence, "Intelligance", colour);

            if (EP.EquipmentSlots[Index].item == null || item.Dexterity == EP.EquipmentSlots[Index].item.Dexterity)
                colour = Color.black;
            else if (item.Dexterity < EP.EquipmentSlots[Index].item.Dexterity)
                colour = Color.red;
            else if (item.Dexterity > EP.EquipmentSlots[Index].item.Dexterity)
                colour = Color.green;
            AddStat(item.Dexterity, "Dexterity", colour);

            if (EP.EquipmentSlots[Index].item == null || item.Agility == EP.EquipmentSlots[Index].item.Agility)
                colour = Color.black;
            else if (item.Agility < EP.EquipmentSlots[Index].item.Agility)
                colour = Color.red;
            else if (item.Agility > EP.EquipmentSlots[Index].item.Agility)
                colour = Color.green;
            AddStat(item.Agility, "Agility", colour);

            if (EP.EquipmentSlots[Index].item == null || item.PhysicalDamage == EP.EquipmentSlots[Index].item.PhysicalDamage)
                colour = Color.black;
            else if (item.PhysicalDamage < EP.EquipmentSlots[Index].item.PhysicalDamage)
                colour = Color.red;
            else if (item.PhysicalDamage > EP.EquipmentSlots[Index].item.PhysicalDamage)
                colour = Color.green;
            AddStat(item.PhysicalDamage, "Physical Damage", colour);

            if (EP.EquipmentSlots[Index].item == null || item.Defence == EP.EquipmentSlots[Index].item.Defence)
                colour = Color.black;
            else if (item.Defence < EP.EquipmentSlots[Index].item.Defence)
                colour = Color.red;
            else if (item.Defence > EP.EquipmentSlots[Index].item.Defence)
                colour = Color.green;
            AddStat(item.Defence, "Defence", colour);

        }
        else
        {
            if(item.type == ItemType.HealthPotion)
            AddStat(item.AmountToAdd, "+ Health Points", colour = Color.green);
            else if (item.type == ItemType.ManaPotioin)
                AddStat(item.AmountToAdd, "+ Mana Points", colour = Color.green);
        }
        ItemStatText.text = sb.ToString();
        if(!PlayerSlot)
        {
            if (PlayerRef.Money > item.ItemPrice)
                ItemPrice.text = ("<color=#" + ColorUtility.ToHtmlStringRGBA(Color.yellow) + ">" + item.ItemPrice + "</color>");
            else
                ItemPrice.text = ("<color=#" + ColorUtility.ToHtmlStringRGBA(Color.red) + ">" + item.ItemPrice + "</color>");
        }
        else
            ItemPrice.text = ("<color=#" + ColorUtility.ToHtmlStringRGBA(Color.yellow) + ">" + item.ItemPrice + "</color>");
    }
    
    private void AddStat(int Value, string StatName, Color colour)
    {
        if (Value != 0)
        {
            //Color colour = Color.green;
            sb.Append("<color=#" + ColorUtility.ToHtmlStringRGBA(colour) + ">" + Value + "</color>");
            sb.Append(" ");
            sb.Append("<color=#" + ColorUtility.ToHtmlStringRGBA(colour) + ">" + StatName + "</color>" + "   ");
        }
    }

    public void CheckItemStack()
    {
        if(PlayerSlot)
        {
            if (Item != null)
            {
                if (Item.StackAmount > 0)
                    AmountText.text = Item.StackAmount.ToString();
                else
                    AmountText.text = "";

            }
        }        
        //inventory.CheckStackNum -= CheckItemStack;
    }

    
    private void Start()
    {
        PlayerRef = GameObject.Find("Player").GetComponent<BasePlayer>();
        EP = GameObject.Find("CharacterPanel").GetComponent<EquipmentPanel>();
    }
 
    public void OnPointerClick(PointerEventData eventData)
    {                       
        if (eventData != null && eventData.button == PointerEventData.InputButton.Left)
        {
            Items item = Item as Items;

            if (PlayerSlot)
            {
                inventory.SellItem(item);
            }
            else
            {
                inventory.BuyItem(item);
            }
        }              
    }
   
    protected virtual void OnValidate()
    {
        if(Image == null)
        {

            if (IsEquipmentSlot)
                Image = GetComponent<Image>();
            else
                Image = gameObject.transform.Find("Slot2").GetComponent<Image>();
        }        
        AmountText = GetComponentInChildren<Text>();
        if(!GetComponent<EquipmentSlots>())
        {
            ItemNameText = transform.Find("ItemNameText").GetComponent<Text>();
            ItemStatText = transform.Find("ItemStatText").GetComponent<Text>();
            ItemDescriptionText = transform.Find("ItemDescriptionText").GetComponent<Text>();
        }       
    }
    
}
