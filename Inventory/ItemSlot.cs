using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public event Action<Items> OnRightClickEvent;

    public Image Image;
    public Items item;
    [SerializeField] ItemToolTip toolTip;
    public Inventory inventory;
    private bool AtShop = false;
    private BasePlayer PlayerRef;
    [SerializeField] Text AmountText;
    public bool IsEquipmentSlot;
    public float Timer;
    public bool StartTimer;
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
            //if (IsPotionSlot == false)
            //{
                
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
           // }
            
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
            if (item.type == ItemType.HealthPotion)
                AddStat(item.AmountToAdd, "+ Health Points", colour = Color.green);
            else if (item.type == ItemType.ManaPotioin)
                AddStat(item.AmountToAdd, "+ Mana Points", colour = Color.green);
        }
        ItemStatText.text = sb.ToString();
        ItemPrice.text = "Price: " + item.ItemPrice;
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
        if(Item != null)
        {
            if (Item.StackAmount > 0)
                AmountText.text = Item.StackAmount.ToString();
            else
                AmountText.text = "";
            
        }
        inventory.CheckStackNum -= CheckItemStack;
    }

    
    private void Start()
    {
        PlayerRef = GameObject.Find("Player").GetComponent<BasePlayer>();
        EP = GameObject.Find("CharacterPanel").GetComponent<EquipmentPanel>();
        //PlayerRef.PlayerCanBuy += InShop;
        //PlayerRef.PlayerCantBuy += OutOfShop;
    }
 
    public void OnPointerClick(PointerEventData eventData)
    {       
        if (ItemDropped)
        {
            ItemDropped = false;
            return;
        }                 
        if (eventData != null && eventData.button == PointerEventData.InputButton.Left)
        {
            EquippableItems equippableItem = Item as EquippableItems;

            if(!AtShop)
            {
                if (equippableItem != null && OnRightClickEvent != null)
                {
                    OnRightClickEvent(equippableItem);                   
                }
                else
                {         
                    if(item.type != ItemType.Miscellaneous)
                    {
                        Item.Use(Item);
                        inventory.RemoveItem(Item);
                    }                   
                }
                StartTimer = false;//quick fix
                Timer = 0;//quick fix
            }
            else if(AtShop && item.Equipped == false)
            {
                //SkillManager.Instance.Money += item.ItemPrice;
                inventory.RemoveItem(item);
                //UIManager.instance.UpdateMoney();
            }
        }              
    }
    public void Down()
    {
        StartTimer = true;
    }
    private void Update()
    {
        if(StartTimer)
        {
            if(Input.GetMouseButtonUp(0))
            {
                StartTimer = false;
                Timer = 0;
                return;
            }
            Timer += Time.deltaTime;
            if (Timer >= 0.5f)
            {
                if (item is Items && item.type != ItemType.Miscellaneous)
                {
                    ItemManager.instance.DropItem(item);
                    inventory.RemoveItem(item);
                    Timer = 0;
                    ItemDropped = true;
                    StartTimer = false;
                    return;
                }
            }
            /*
            else if (Timer >= 5)
            {
                StartTimer = false;//make sure timer isnt going on forever;
            }
            */
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
        if (toolTip == null)
        {
            toolTip = FindObjectOfType<ItemToolTip>();
        }
        AmountText = GetComponentInChildren<Text>();
        if(!GetComponent<EquipmentSlots>())
        {
            ItemNameText = transform.Find("ItemNameText").GetComponent<Text>();
            ItemStatText = transform.Find("ItemStatText").GetComponent<Text>();
            ItemDescriptionText = transform.Find("ItemDescriptionText").GetComponent<Text>();
        }       
    }
    
    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        Items aItem = Item as Items;

        if(aItem != null)
        {
            toolTip.ShowToolTip(aItem);
        }  
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.HideToolTip();
    }
    */
    public void InShop()
    {
        AtShop = true;
    }
    public void OutOfShop()
    {
        AtShop = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Down();
    }
}
