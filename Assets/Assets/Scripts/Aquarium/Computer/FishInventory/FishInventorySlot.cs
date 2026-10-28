using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishInventorySlot : MonoBehaviour
{
    [SerializeField] private Image fishIcon;
    [SerializeField] private TMP_Text fishName;

    public void SetFish(Fish fish)
    {
        if (fish == null) return;

        fishIcon.sprite = fish.sprite;
        fishName.text = fish.fishName;
        fishIcon.enabled = fish.sprite != null;
    }
}
