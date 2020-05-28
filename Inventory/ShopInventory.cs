using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ShopInventory : MonoBehaviour
{
    public static ShopInventory Instance;
    public delegate void UpdateStackNumber();
    public event UpdateStackNumber CheckStackNum;

    public List<Items> ShopItems;
    [SerializeField] Transform PlayerItemsParent, ShopItemsParent;
    public  ShopItemSlot[] PlayerItemSlots, ShopItemSlots;
    public event Action<Items> OnItemRightClickEvent;
    private int i = 0;
    public Text MoneyText;
    private BasePlayer PlayerRef;
    public ShopMerchant Sender;
    private void Start()
    {
        Instance = this;
        PlayerRef = GameObject.Find("Player").GetComponent<BasePlayer>();
        if (PlayerItemsParent != null)
        {
            PlayerItemSlots = PlayerItemsParent.GetComponentsInChildren<ShopItemSlot>();
        }
        if (ShopItemsParent != null)
        {
            ShopItemSlots = ShopItemsParent.GetComponentsInChildren<ShopItemSlot>();
        }

        for (int i = 0; i < PlayerItemSlots.Length; i++)
        {
            PlayerItemSlots[i].OnRightClickEvent += OnItemRightClickEvent;
            /*
            if (PlayerItemSlots[i].Item != null)
            {
                PlayerItemSlots[i].Item.player = PlayerRef;
            }
             */
        }
        for (int i = 0; i < ShopItemSlots.Length; i++)
        {
            ShopItemSlots[i].OnRightClickEvent += OnItemRightClickEvent;
            /*
            if (ShopItemSlots[i].Item != null)
            {
                ShopItemSlots[i].Item.player = PlayerRef;
            }
            */
        }
    }
    public void CloseShop()
    {
        PlayerRef.Controller.InventoryOpen = false;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if(PlayerRef != null)
        {
            PlayerRef.Controller.InventoryOpen = true;
            SetMoneyText();
            /*
            foreach(Items T in PlayerRef.inventory.Items)
            {
                if (T.ShopItem == Sender.SellingType)
                {
                    PlayerItems.Add(T);
                }
                    
            }
            */
            RefreshUI();
        }        
        
    }
    private void SetMoneyText() { MoneyText.text = "Money: " + PlayerRef.Money; }
    public void RefreshUI()
    {
        //int i = 0;
        int o = 0;
        for(int i = 0; i < PlayerRef.inventory.Items.Count && i < PlayerItemSlots.Length; i++)
        {
            if(PlayerRef.inventory.Items[i].ShopItem == Sender.SellingType)
            {
                PlayerItemSlots[o].gameObject.SetActive(true);
                PlayerItemSlots[o].Item = PlayerRef.inventory.Items[i];
                PlayerItemSlots[o].CheckItemStack();
                o++;
            }      
            else
            {
                PlayerItemSlots[i].gameObject.SetActive(false);
            }
                
        }
        for (i = o; i < PlayerItemSlots.Length; i++)
        {
            PlayerItemSlots[i].Item = null;
            PlayerItemSlots[i].gameObject.SetActive(false);
        }
        /*
        for (; i < PlayerItemSlots.Length; i++)
        {
            PlayerItemSlots[i].Item = null;
            PlayerItemSlots[i].gameObject.SetActive(false);
        }
        */
        int j = 0;
        for (; j < ShopItems.Count && j < ShopItemSlots.Length; j++)
        {
            ShopItemSlots[j].gameObject.SetActive(true);
            ShopItemSlots[j].Item = ShopItems[j];
            ShopItemSlots[j].CheckItemStack();
        }
        for (; j < ShopItemSlots.Length; j++)
        {
            ShopItemSlots[j].Item = null;
            ShopItemSlots[j].gameObject.SetActive(false);
        }
    }
    public bool IsFull()
    {
        return PlayerRef.inventory.Items.Count >= PlayerItemSlots.Length;
    }

    public void SellItem(Items item)
    {
        PlayerRef.Money += item.ItemPrice;
        PlayerRef.inventory.RemoveItem(item);
        RefreshUI();
        SetMoneyText();
    }
    public void BuyItem(Items item)
    {
        if(PlayerRef.Money >= item.ItemPrice)
        {
            PlayerRef.Money -= item.ItemPrice;
            item.player = PlayerRef;
            PlayerRef.inventory.AddItem(item);
            SetMoneyText();
            RefreshUI();
        }
    }
}
