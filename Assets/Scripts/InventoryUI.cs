using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inv;
    [SerializeField] private GameObject slotPref;
    [SerializeField] private Transform slotPar;
    [SerializeField] private TextMeshProUGUI money;

    private List<SlotUI> slots = new List<SlotUI>();

    private void Awake()
    {
        GenSlots();
    }

    private void MoneyChanged()
    {
        money.text = inv.money.ToString();
    }

    private void OnEnable()
    {
        // --- Подписываем обновление UI на любые изменения
        if (inv != null) inv.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDisable()
    {
        // --- Отписываем обновление UI на любые изменения
        if (inv != null) inv.OnInventoryChanged -= RefreshUI;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        List<InventoryItem> items = new List<InventoryItem>(inv.GetAllItems());

        foreach (SlotUI slot in slots)
            slot.ClearSlot();
        

        // --- Заполнение актуальными слотами
        for(int i = 0; i < items.Count; i++)
        {
            bool isN = inv.IsNewIt(items[i]);
            slots[i].SetItem(items[i], isN);
        }

        MoneyChanged();
    }

    private void GenSlots()
    {
        int cap = inv.capacity;

        for (int i = 0; i < cap; i++)
        {
            GameObject slotObj = Instantiate(slotPref, slotPar);
            SlotUI slot = slotObj.GetComponent<SlotUI>();

            slot.ClearSlot();
            slots.Add(slot);
        }
    }

    public void SortByName()
    {
        inv.SortName();
        RefreshUI();
    }

    public void SortByType()
    {
        inv.SortType();
        RefreshUI();
    }

    public void SortByCount()
    {
        inv.SortCount();
        RefreshUI();
    }
}
