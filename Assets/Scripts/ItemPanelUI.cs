using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanelUI : MonoBehaviour
{
    [SerializeField] private Image iconObj;
    [SerializeField] private TextMeshProUGUI nameObj;
    [SerializeField] private TextMeshProUGUI descObj;

    private void Awake()
    {
        iconObj.color = new Color(255, 255, 255, 0);

        nameObj.text = "";
        descObj.text = "";
    }

    public void Take(Item item)
    {
        iconObj.sprite = item.icon;
        iconObj.color = new Color(255, 255, 255, 255);

        nameObj.text = item.itemName;
        descObj.text = item.description;
    }
}
