using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Parameters")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantity;
    [SerializeField] private ToolTip ttip;
    [SerializeField] private Inventory inv;
    [SerializeField] private Canvas canvas;

    public InventoryItem currItem;

    private ItemPanelUI itPanel;

    private float lastClckTime;
    private const float dblClckDelay = .3f;

    private GameObject dragIcon;

    private SlotUI hovSlot;

    private static SlotUI draggINGSlot;

    public static SlotUI selSlot;

    [Header("Colors")]
    [SerializeField] private Color baseCol = new Color(0.66f, 0.66f, 0.66f, 1f);
    [SerializeField] private Color selCol = new Color(0f, 0.6f, 0.1f, 1f);
    [SerializeField] private Color newItCol = new Color(1f, 0.9f, 0.6f, 1f);

    private bool isNewIt = false;

    private void Awake()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>(true);
        if (ttip == null) ttip = FindObjectOfType<ToolTip>(true);
        if (inv == null) inv = FindObjectOfType<Inventory>(true);
        if (icon == null) icon = GetComponentInChildren<Image>(true);
        if (quantity == null) quantity = GetComponentInChildren<TextMeshProUGUI>(true);
        if (itPanel == null) itPanel = FindObjectOfType<ItemPanelUI>(true);
    }

    public void SetItem(InventoryItem invItem, bool isNew = false)
    {
        currItem = invItem;
        isNewIt = isNew;

        if (invItem == null)
        {
            ClearSlot();
            return;
        }

        icon.sprite = invItem.item.icon;
        icon.enabled = true;
        // --- Показываем количество если стек больше 1
        quantity.text = invItem.item.isStackable && invItem.quantity > 1 ? invItem.quantity.ToString() : "";

        if (isNewIt) GetComponent<Image>().color = newItCol;
        else GetComponent<Image>().color = baseCol;
    }

    public void ClearSlot()
    {
        currItem = null;
        icon.enabled = false;
        quantity.text = "";
        GetComponent<Image>().color = baseCol;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currItem == null) return;

        itPanel.Take(currItem.item);
        SelItem(this);

        // --- Двойной клик для использования
        if (Time.time - lastClckTime < dblClckDelay)
        {
            inv.UseItem(currItem);
            lastClckTime = 0;
        } else
        {
            lastClckTime = Time.time;
        }
    }

    private void SelItem(SlotUI slot)
    {
        if(selSlot != null && selSlot != slot)
        {
            selSlot.GetComponent<Image>().color = baseCol;
            inv.CleanNewItem(selSlot.currItem);
        }

        selSlot = slot;
        inv.SetSelSlot();

        Debug.Log("Selected");
        slot.GetComponent<Image>().color = new Color(0f, 0.6f, 0.1f, 1f);
        slot.isNewIt = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(hovSlot == this) return;
        hovSlot = this;

        if (currItem != null && ttip != null) ttip.Show(currItem.item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(hovSlot == this) hovSlot = null;

        if(ttip != null) ttip.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(currItem == null || canvas == null) return;

        draggINGSlot = this;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        Image img = dragIcon.AddComponent<Image>();
        img.sprite = icon.sprite;
        img.raycastTarget = false;
        dragIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(150,150);

        Debug.Log(draggINGSlot);

        if (ttip != null) ttip.Hide();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null) dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(dragIcon != null) Destroy(dragIcon);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggINGSlot == null || draggINGSlot == this) return;

        var items = inv.GetAllItems();

        int fromIdx = inv.GetAllItems().IndexOf(draggINGSlot.currItem);
        int toIdx = inv.GetAllItems().IndexOf(currItem);

        if (fromIdx < 0 && toIdx < 0) return;

        InventoryItem fromItm = items[fromIdx];
        InventoryItem toItm = items[toIdx];

        // --- Стак предметов одного типа
        if (fromItm.item.type == toItm.item.type && fromItm.item.isStackable && toItm.item.isStackable)
        {
            int total = fromItm.quantity += toItm.quantity;
            int maxStk = fromItm.item.maxStackSize;

            if(total <= maxStk)
            {
                toItm.quantity = total;
                items.RemoveAt(fromIdx);
            } else
            {
                toItm.quantity = maxStk;
                fromItm.quantity = total - maxStk;
            }

            inv.RefreshFrUI(items);
            return;
        }


        // --- Если разные или не стакуются
        (items[fromIdx], items[toIdx]) = (items[toIdx], items[fromIdx]);
        inv.RefreshFrUI(items);

        Debug.Log("Dropped");
    }
}
