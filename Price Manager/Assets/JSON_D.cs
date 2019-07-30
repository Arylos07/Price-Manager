using UnityEngine;
using LitJson;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class parseJSON
{
    public string name;
    public string salePrice;
    public string sku;
    public ArrayList but_name;
    public ArrayList but_salePrice;
}
public class JSON_D : MonoBehaviour
{
    public string APIKey;
    //storing SKU's (int) and price (string)
    public Dictionary<string, string> prices;
    //storing SKU's (int) and names (string)
    public Dictionary<string, string> itemNames;

    public Image loadingBar;
    public Text progressText;

    public GameObject itemField;
    public GameObject panel;
    public GameObject contentView;

    public Text timeEstimate;
    public Text totalSkus;
    public Text updatedSkus;
    public Text accuracy;

    public GameObject failedSKU;
    public GameObject failedSKUContent;

    public Text status;

    [HideInInspector]
    public List<string> keys = new List<string>();   //used as stage for keys to remove "USD" prefix from prices

    //storing updated prices for SKU's (int), price (string)
    public Dictionary<string, string> updates = new Dictionary<string, string>();

    //storing approved updates, SKU's (int), price(string)
    public Dictionary<string, string> approvedUpdates = new Dictionary<string, string>();

    IEnumerator Start()
    {

        status.text = "Initializing...";

        timeEstimate.text = string.Empty;
        string fileData = File.ReadAllText(Application.persistentDataPath + "/products_export.csv");
        //Debug.Log(fileData);
        string[] lines = fileData.Split("\n"[0]);
        //Debug.Log(lines[2]);
        string[] lineData = (lines[1].Trim()).Split(","[0]);
        //Debug.Log(lineData[13]);

        //run foreach for each line. Linedata[15] is SKU, 3 is price (remember to format without USD in list and reformat with USD when reimporting), 1 is name, 

        prices = new Dictionary<string, string>();
        itemNames = new Dictionary<string, string>();

        yield return new WaitForSeconds(1);

        status.text = "Loading SKU's...";

        yield return new WaitForSeconds(1);
        int i = 1;

        foreach (string line in lines)
        {
            if (i < lines.Length - 1)
            {
                string name = string.Empty;
                string SKU = string.Empty;
                string price = string.Empty;


                lineData = (lines[i].Trim()).Split(","[0]);

                if (lineData[13].ToString() != string.Empty)
                {

                    name = lineData[1].ToString();
                    SKU = lineData[13].ToString();
                    price = lineData[19].ToString();

                    prices.Add(SKU, price);
                    itemNames.Add(SKU, name);

                }

                i++;
                status.text = "SKU's loaded: " + prices.Count.ToString();
            }

            yield return new WaitForSeconds(0.05f);
        }
        foreach (KeyValuePair<string, string> pair in prices)
        {
            keys.Add(pair.Key);
        }

        totalSkus.text = "Total SKU's: " + keys.Count;

        StartCoroutine(PricingUpdate());

        yield return null;
    }

    public IEnumerator PricingUpdate()
    {
        status.text = "Initializing PricingUpdate()...";
        int progress = 0;
        int intProgress = 0;

        yield return new WaitForSeconds(0.5f);

        status.text = "Getting pricing updates";

        foreach (string sku in keys)
        {
            string url = "https://api.bestbuy.com/v1/products/" + sku + ".json?apiKey=" + APIKey;
            WWW www = new WWW(url);
            yield return www;
            if (www.error == null)
            {
                //print(www.text);
                Processjson(www.text);
            }
            else
            {
                GameObject failed = Instantiate(failedSKU, failedSKUContent.transform);
                failed.GetComponent<Text>().text = sku + " - " + www.error;
                failed = null;
            }

            progress++;
            intProgress++;

            loadingBar.fillAmount = ((float)progress / (float)keys.Count);
            progressText.text = (((float)progress / (float)keys.Count) * 100).ToString("F2") + "%";

            int timer = (keys.Count - progress);

            float minutes = Mathf.Floor((float)timer / 60);
            float seconds = Mathf.RoundToInt((float)timer % 60);

            timeEstimate.text = "Est time: " + minutes + " min, " + seconds + " sec";

            if(intProgress >= 10)
            {
                intProgress = 0;
                status.text = "Getting pricing updates";
            }
            status.text = status.text + ".";

            yield return new WaitForSeconds(1);
        }

        if (progress == keys.Count)
        {
            status.text = "Finalizing list...";

            foreach (KeyValuePair<string, string> item in updates)
            {
                GameObject instance = Instantiate(itemField, contentView.transform);
                instance.GetComponent<UpdateSKU>().name = itemNames[item.Key];
                instance.GetComponent<UpdateSKU>().SKU = item.Key;
                instance.GetComponent<UpdateSKU>().Price = item.Value;
                instance.GetComponent<UpdateSKU>().dataManager = gameObject.GetComponent<JSON_D>();
                instance.GetComponent<UpdateSKU>().Init();

                instance = null;
            }
            yield return new WaitForSeconds(1);
            panel.SetActive(false);
            status.text = string.Empty;

            if (updates.Count == 0)
            {
                status.text = "No prices needing updating";
            }

            else
            {
                status.text = "Price update complete: " + updates.Count + " SKU's to review.";
            }
        }
    }

    private void Processjson(string jsonString)
    {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        parseJSON parsejson;
        parsejson = new parseJSON();
        parsejson.name = jsonvale["name"].ToString();
        parsejson.salePrice = jsonvale["salePrice"].ToString();
        parsejson.sku = jsonvale["sku"].ToString();

        if (parsejson.salePrice != prices[parsejson.sku])
        {
            //print("Update Price: " + parsejson.sku + ". Old price - " + prices[parsejson.sku] + ", new price - " + parsejson.salePrice);
            updates.Add(parsejson.sku, parsejson.salePrice);
            updatedSkus.text = "SKU's updated: " + updates.Count.ToString();
            accuracy.text = "Accuracy: " + (((float)(keys.Count - updates.Count) / (float)keys.Count) * 100).ToString("F0") + "%";
        }
    }
}