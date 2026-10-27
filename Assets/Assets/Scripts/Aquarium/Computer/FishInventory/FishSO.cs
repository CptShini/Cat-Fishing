using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "ScriptableObjects/FishSO")]
public class FishSO : ScriptableObject
{
    public string fishName;
    public Sprite fishIcon;
}