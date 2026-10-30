using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "ScriptableObjects/FishSO")]
public class FishSO : ScriptableObject
{
    public string fishKey;
    public string fishName;

    public Sprite fishIcon;

    public float defaultStrength;

    [Header("Weight")]
    public float weightMin;
    public float weightMax;

}