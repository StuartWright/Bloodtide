using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMerchant : BaseNPC
{
    public ShopType SellingType;
    public List<Items> ItemsToSell = new List<Items>();
    private ShopInventory SI;
    private ItemManager IM;
    public bool GeneralStore;
    private new void Start()
    {
        SI = ShopInventory.Instance;
        IM = ItemManager.instance;
        if(!GeneralStore)
        {
            Player = GameObject.Find("Player").GetComponent<BasePlayer>();
            Player.LeveledUp += ResetItems;
        }        
        base.Start();
    }
    public override void PlayerStopped()
    {
        if(!GeneralStore && ItemsToSell.Count == 0)
        {
            ItemManager.instance.maxValue = Level;
            FillShop();
        }
        SI.ShopItems = ItemsToSell;
        SI.Sender = this;
        SI.gameObject.SetActive(true);
    }

    public void ResetItems()
    {
        ItemsToSell.Clear();
    }
    private void FillShop()
    {
        List<Items> SellableItems = new List<Items>();
        foreach (GameObject item in IM.Items)
        {
            ItemPickup CurrentItem = item.GetComponent<ItemPickup>();
            if (CurrentItem.item.ShopItem == SellingType)
            {
                SellableItems.Add(CurrentItem.item);                
            }
        }

        for(int i = 0; i < 10; i++)
        {
            Items newItem = IM.createItem(SellableItems[Random.Range(0,SellableItems.Count)]);
            ItemsToSell.Add(newItem);
        }
        
    }
}
