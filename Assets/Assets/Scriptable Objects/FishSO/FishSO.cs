using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "FishSO")]
public class FishSO : ScriptableObject
{
    public string fishName;

    public Sprite fishIcon;

    [Header("Weight")]
    public float weightMin;
    public float weightMax;

}