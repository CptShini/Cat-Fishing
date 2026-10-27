using UnityEngine;

public enum ItemType
{
    MapDecoration,
    TankDecoration
}

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
}
