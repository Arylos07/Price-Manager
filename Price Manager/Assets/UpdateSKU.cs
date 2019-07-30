using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSKU : MonoBehaviour
{
    public JSON_D dataManager;
    public string SKU;
    public string Price;

    public GameObject[] toggles;

    public Toggle toggle;
    public Text itemName;
    public Text sku;
    public Text oldPrice;
    public Text newPrice;

    public void Init()
    {
        itemName.text = dataManager.itemNames[SKU];
        sku.text = SKU.ToString(); ;
        oldPrice.text = "$" + dataManager.prices[SKU];
        newPrice.text = "$" + Price;
    }

    public void Approve(bool approved)
    {
        if (approved == true)
        {
            dataManager.approvedUpdates.Add(SKU, Price);
        }
        else if (approved == false)
        {
            if (dataManager.approvedUpdates.ContainsKey(SKU))
            {
                dataManager.approvedUpdates.Remove(SKU);
            }
        }
    }

    public void Select(bool select)
    {
        toggle.isOn = select;
    }

    public void SelectAll(bool select)
    {
        toggles = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject toggle in toggles)
        {
            toggle.GetComponent<UpdateSKU>().Select(select);
        }
    }
}
