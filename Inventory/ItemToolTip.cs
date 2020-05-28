using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ItemToolTip : MonoBehaviour
{
    //[SerializeField] Text ItemSlotText;
    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemStatText;    
    [SerializeField] Text ItemDescriptionText;
    [SerializeField] Text ItemPrice;

    private StringBuilder sb = new StringBuilder();

    private void Start()
    {
        HideToolTip();
    }

    public void ShowToolTip(Items item)
    {
        ItemNameText.text = item.ItemName;
       // ItemSlotText.text = item.EquipmentType.ToString();
        ItemDescriptionText.text = "Description: " + item.ItemDescription;
        sb.Length = 0;

        AddStat(item.Strength, "Strength");
        AddStat(item.Intelligence, "Intelligance");
        AddStat(item.Dexterity, "Stamina");
        AddStat(item.Agility, "Agility");
        AddStat(item.PhysicalDamage, "Physical Damage");
       // AddStat(item.MagicDamage, "Magic Damage");
        
        ItemStatText.text = sb.ToString();
        ItemPrice.text = "Price: " + item.ItemPrice;
       // gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        // gameObject.SetActive(false);
     ItemNameText.text = "";
     ItemStatText.text = "";
    // ItemSlotText.text = "";
     ItemDescriptionText.text = "";
     ItemPrice.text = "";
    }

    private void AddStat(int Value, string StatName)
    {
        if(Value != 0)
        {
            if(sb.Length > 0)
            {
                sb.AppendLine();
            }
            if(Value > 0)
            {
                sb.Append("+");
            }

            sb.Append(Value);
            sb.Append(" ");
            sb.Append(StatName);
        }
    }
}
