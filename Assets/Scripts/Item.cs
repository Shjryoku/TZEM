using UnityEngine;

public enum ItemType { Weapon, Armor, Potion, Food, Questionable, Scrolls }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public Sprite icon;
    public string description;
    public bool isStackable;
    public int maxStackSize;
}
