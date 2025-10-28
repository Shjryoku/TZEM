using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private GameObject ttipPan;
    [SerializeField] private TextMeshProUGUI iName;

    private Item currItem;

    private void Awake()
    {
        Hide();
    }

    public void Show(Item item)
    {
        if (item == null) return;
        if (ttipPan.activeSelf && currItem == item) return;

        // --- ¬первые наводим на объект. —юда мы в дальнейшем при удержании курсора на объекте не должнеы входить
        currItem = item;
        ttipPan.SetActive(true);

        //RectTransform rcTransform = ttipPan.GetComponent<RectTransform>();

        //Vector2 pos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    ttipPan.transform.parent as RectTransform,
        //    Input.mousePosition,
        //    null,
        //    out pos);

        //rcTransform.anchoredPosition = pos + new Vector2(0, 50);

        ttipPan.transform.position = Input.mousePosition + new Vector3(0, 50, 0);

        iName.text = item.itemName;
    }

    public void Hide()
    {
        currItem = null;
        ttipPan.SetActive(false);
    }
}
