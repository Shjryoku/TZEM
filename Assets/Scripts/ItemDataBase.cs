using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Inventory/Item Database")]
public class ItemDataBase : ScriptableObject
{
    public List<ItmEntry> items;

    public Item GetItemById(string id)
    {
        return items.FirstOrDefault(e => e.id == id).item;
    }
}