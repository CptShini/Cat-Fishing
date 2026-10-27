using UnityEngine;

[CreateAssetMenu(fileName = "NewDecorItem", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string decorName;
    public Sprite decorIcon;
}
