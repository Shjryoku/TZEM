using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI maxTxt;
    [SerializeField] private TextMeshProUGUI minTxt;
    [SerializeField] private Inventory inv;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();

        // --- Подписываем на изменения слайдера
        slider.onValueChanged.AddListener(UpdateMinTxt);
    }

    private void OnDestroy()
    {
        // --- Отписываем от изменений слайдера
        slider.onValueChanged.RemoveListener(UpdateMinTxt);
    }

    private void UpdateMinTxt(float value)
    {
        minTxt.text = ((int)value).ToString();
    }

    public void SetMax(int max)
    {
        maxTxt.text = max.ToString();

        slider.maxValue = max;
        UpdateMinTxt(slider.value);
    }

    public void ToDrop()
    {
        inv.RemItem(inv.GetSelected().currItem, ((int)slider.value));
        inv.CancelDropPan();
        inv.Notify();
    }
}
