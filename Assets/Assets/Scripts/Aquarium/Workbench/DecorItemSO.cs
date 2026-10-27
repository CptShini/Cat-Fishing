using UnityEngine;

[CreateAssetMenu(fileName = "NewDecorItem", menuName = "ScriptableObjects/DecorItemSO")]
public class DecorItemSO : ScriptableObject
{
    public string decorName;
    public Sprite decorIcon;
}
