using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Inventory : MonoBehaviour
{
    public delegate void UpdateStackNumber();
    public event UpdateStackNumber CheckStackNum;

    public List<Items> Items;
    [SerializeField] Transform ItemsParent;
    public  ItemSlot[] ItemSlots;
    public event Action<Items> OnItemRightClickEvent;
    private int i = 0;
    private BasePlayer PlayerRef;
    private void Start()
    {
        PlayerRef = GameObject.Find("Player").GetComponent<BasePlayer>();
        if (ItemsParent != null)
        {
            ItemSlots = ItemsParent.GetComponentsInChildren<ItemSlot>();
        }
        

        for (int i = 0; i < ItemSlots.Length; i++)
        {
            ItemSlots[i].OnRightClickEvent += OnItemRightClickEvent;
            
            if (ItemSlots[i].Item != null)
            {
                ItemSlots[i].Item.player = PlayerRef;
            }
             
        }
        RefreshUI();
    }

   
    public void RefreshUI()
    {
        int i = 0;
        for(; i < Items.Count && i < ItemSlots.Length; i++)
        {
            ItemSlots[i].gameObject.SetActive(true);
            ItemSlots[i].Item = Items[i];
            ItemSlots[i].CheckItemStack();
            //CheckStackNum += ItemSlots[i].CheckItemStack;
            //CheckStackNum();
        }
        for (; i < ItemSlots.Length; i++)
        {
            ItemSlots[i].Item = null;
            ItemSlots[i].gameObject.SetActive(false);
        }
    }

    public void AddItem(Items item)
    {     
        if (item.CanStack)
        {
            bool HasStacked = false;
            if(Items.Count > 0)
            {
                for (i = 0; i < Items.Count; i++)
                {
                    if (item.ItemName == Items[i].ItemName)
                    {
                        HasStacked = true;
                        Items[i].StackAmount++;
                        //ItemSlots[i].CheckItemStack();//////////////////////////////
                        break;
                    }         
                }
            }
            if(!HasStacked)
            {
                Items.Add(item);
            }
        }
      else
        {
            Items.Add(item);
        }
        RefreshUI();

    }

    public bool IsFull()
    {
        return Items.Count >= ItemSlots.Length;
    }

    public bool RemoveItem(Items item)
    {       
        if (item.StackAmount <= 1)
        {
            Items.Remove(item);
            RefreshUI();
            return true;
        }
        else
        {
            item.StackAmount--;
            RefreshUI();
        }
        return false;
    }

    public void UseHealthPotion()
    {
        Items item = null;
        float value = 0;
        float PlayerHP = Mathf.Abs(PlayerRef.Health - PlayerRef.MaxHealth);
        for(int i = 0; i < Items.Count; i++)
        {
            if (Items[i].type == ItemType.HealthPotion)
            {
               if(Items[i].AmountToAdd > value && Items[i].AmountToAdd <= PlayerHP)
                {
                    item = Items[i];
                    value = Items[i].AmountToAdd;
                }
            }
        }
        if(PlayerRef.Health < PlayerRef.MaxHealth)
        {
            if(item == null)
            {
                value = Mathf.Infinity;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].type == ItemType.HealthPotion)
                    {
                        if (Items[i].AmountToAdd < value)
                        {
                            item = Items[i];
                            value = Items[i].AmountToAdd;
                        }
                    }
                }
                
            }
            if(item != null)
            {
                item.Use(item);
                RemoveItem(item);
            }
        }
    }
    public void UseManaPotion()
    {
        Items item = null;
        float value = 0;
        float PlayerMP = Mathf.Abs(PlayerRef.Mana - PlayerRef.MaxMana);
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].type == ItemType.ManaPotioin)
            {
                if (Items[i].AmountToAdd > value && Items[i].AmountToAdd <= PlayerMP)
                {
                    item = Items[i];
                    value = Items[i].AmountToAdd;
                }
            }
        }
        if (PlayerRef.Mana < PlayerRef.MaxMana)
        {
            if (item == null)
            {
                value = Mathf.Infinity;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].type == ItemType.ManaPotioin)
                    {
                        if (Items[i].AmountToAdd < value)
                        {
                            item = Items[i];
                            value = Items[i].AmountToAdd;
                        }
                    }
                }               
            }     
            if(item != null)
            {
                item.Use(item);
                RemoveItem(item);
            }
        }
    }
    public void CheckQuestItem(Items item, Quest quest)
    {
        for(int i = 0; i < Items.Count; i++)
        {
            if(Items[i].ItemName == item.ItemName)
            {
                quest.CurrentAmount = Items[i].StackAmount;
            }
        }
    }
    public void RemoveQuestItem(Items item, Quest quest)
    {
        quest.RemoveQuestItem -= RemoveQuestItem;
        for(int i = 0; i < Items.Count; i++)
        {
            if(Items[i].ItemName == item.ItemName)
            {
                Items.Remove(Items[i]);
                RefreshUI();
                break;
            }
        }
    }
}
