using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity;

    public InventoryItem(Item item, int amount = 1)
    {
        this.item = item;
        this.quantity = amount;
    }
}

public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    public int capacity => rows * columns;

    [SerializeField] private ItemDataBase dataBase;
    private bool isLoaded;

    [SerializeField] private GameObject dropPan;
    [SerializeField] private DropPanel dropPanClss;

    [Header("Current items")]
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>(); // --- Лист для содержания предметов

    public List<InventoryItem> newIts = new List<InventoryItem>();

    public event Action OnInventoryChanged; // --- Ивент система для сигналов об изменении инвентаря

    private SlotUI selected;

    public int money;


    public bool isDropping {  get; private set; } = false;

    private void Awake()
    {
        dropPan.SetActive(false);

        if (dataBase != null)
        {
            var loaded = SaveSystem.LoadInv(dataBase);
            if (loaded != null && loaded.Count > 0) items = loaded;
        }

        isLoaded = true;
        Notify();
    }

    private void Start()
    {
        // --- Синхронизация новых предметов с основным списком
        foreach (var itm in newIts)
            if(!items.Contains(itm)) items.Add(itm);

        Notify();
    }

    public void RefreshFrUI(List<InventoryItem> newList)
    {
        items = newList;
        Notify();
    }

    // --- Заполняем существующие стеки, потом создаём новый
    public bool AddItem(Item item, int amount = 1)
    {
        if(item == null) return false;

        bool added = false;

        if (item.isStackable)
        {
            int remaining = amount;

            foreach (InventoryItem exstItem in items)
            {
                if (exstItem.item == item)
                {
                    int space = item.maxStackSize - exstItem.quantity;
                    if (space > 0)
                    {
                        int toAdd = Mathf.Min(space, remaining);
                        exstItem.quantity += toAdd;
                        remaining -= toAdd;
                        added = true;

                        if (remaining <= 0)
                        {
                            newIts.Add(exstItem);
                            Notify();
                            return true;
                        }
                    }
                }
            }

            // --- Помечаем предмет новым после добавления
            while(remaining > 0)
            {
                if(items.Count >= capacity)
                {
                    Debug.Log("Inventory full");
                    Notify();
                    return false;
                }

                int toAdd = Mathf.Min(item.maxStackSize, remaining);
                InventoryItem newIt = new InventoryItem(item, toAdd);
                items.Add(newIt);
                newIts.Add(newIt);
                remaining -= toAdd;
                added = true;
            }
        }
        else
        {
            if (items.Count >= capacity)
            {
                Debug.Log("Inventory full");
                return false;
            }

            InventoryItem newIt = new InventoryItem(item, 1);
            items.Add(newIt);
            newIts.Add(newIt);
            added = true;
        }

        if(added) Notify();

        return true;
    }

    public void RemItem(InventoryItem item, int amount = 1)
    {
        InventoryItem invItem = items.Find(i => i == item);
        if (invItem == null) return ;

        if (invItem.item.isStackable)
        {
            invItem.quantity -= amount;
            if (invItem.quantity <= 0) items.Remove(invItem);
        }
        else items.Remove(invItem);

        if(selected != null && selected.currItem == item)
        {
            selected = null;
            SlotUI.selSlot = null;
        }
        
        if(!isDropping) Notify();
    }

    public bool HasItem(InventoryItem item, int amount = 1)
    {
        if (item == null) return false;
        InventoryItem invItem = items.Find(i => i == item);
        if (invItem == null) return false;

        return invItem.item.isStackable ? invItem.quantity >= amount : true;
    }

    public void UseItem(InventoryItem slot)
    {
        if(!HasItem(slot, 1)) return;

        Debug.Log($"Used {slot.item.itemName}");

        if (selected != null && selected.currItem == slot)
        {
            selected = null;
            SlotUI.selSlot = null;
        }

        RemItem(slot, 1);
        Notify();
    }

    public void DropSelItem()
    {
        if (selected == null || selected.currItem == null) return;

        InventoryItem curr = selected.currItem;

        if (selected.currItem.quantity >= 5)
        {
            dropPan.SetActive(true);
            dropPanClss.SetMax(curr.quantity);
            isDropping = true;
        }
        else
        {
            RemItem(curr);

            if (selected != null && selected.currItem == curr)
            {
                selected = null;
                SlotUI.selSlot = null;
            }
        }
    }

    public void CancelDropPan()
    {
        dropPan.SetActive(false);
        isDropping = false;
    }

    public void SetSelSlot()
    {
        if (selected == null || selected.currItem == null) return;

        selected = SlotUI.selSlot;
    }

    public void UseSelectedItem()
    {
        if (selected == null || selected.currItem == null) return;

        UseItem(selected.currItem);
    }

    public void ClearInv()
    {
        items.Clear();
        Notify();
    }

    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(items);
    }

    public void SortName() => items.Sort((a,b) => a.item.itemName.CompareTo(b.item.itemName));
    public void SortType() => items.Sort((a, b) => a.item.type.CompareTo(b.item.type));

    public void SortCount() => items.Sort((a,b) => a.quantity.CompareTo(b.quantity));

    public bool IsNewIt(InventoryItem invIt)
    {
        return newIts.Contains(invIt);
    }

    public void CleanNewItem(InventoryItem invIt)
    {
        if(newIts.Contains(invIt)) newIts.Remove(invIt);
    }

    public SlotUI GetSelected()
    {
        return selected;
    }

    // --- Эвент после изменений инвентаря
    public void Notify()
    {
        OnInventoryChanged?.Invoke();

        if (isLoaded) SaveSystem.SaveInv(GetAllItems());
    }
}
