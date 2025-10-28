using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private const string saveKey = "InventoryData";

    public static void SaveInv(List<InventoryItem> items)
    {
        List<SaveData> saveList = new List<SaveData>();

        foreach (InventoryItem invIt in items)
        {
            saveList.Add(new SaveData
            {
                itemID = invIt.item.itemName,
                quantity = invIt.quantity
            });
        }

        string json = JsonUtility.ToJson(new SerializationWr<SaveData> { list = saveList }, true);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();

        Debug.Log("Saved");
    }

    public static List<InventoryItem> LoadInv(ItemDataBase dataBase)
    {
        List<InventoryItem> items = new List<InventoryItem>();

        if(!PlayerPrefs.HasKey(saveKey)) return items;

        string json = PlayerPrefs.GetString(saveKey);
        SerializationWr<SaveData> wr = JsonUtility.FromJson<SerializationWr<SaveData>>(json);
        if (wr == null || wr.list == null) return items;

        foreach(var data in wr.list)
        {
            Item itmSO = dataBase.GetItemById(data.itemID);
            if(itmSO != null)
            {
                InventoryItem invIt = new InventoryItem(itmSO, data.quantity);
                items.Add(invIt);
            } else Debug.Log($"Item {data.itemID} not found");
        }

        return items;
    }
}

[Serializable]
public class SaveData
{
    public string itemID;
    public int quantity;
}

[Serializable]
public struct ItmEntry
{
    public string id;
    public Item item;
}

[Serializable]
public class SerializationWr<T> { public List<T> list; }
