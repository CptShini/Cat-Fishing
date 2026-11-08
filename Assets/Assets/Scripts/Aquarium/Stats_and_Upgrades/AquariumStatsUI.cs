using UnityEngine;
using TMPro;

public class AquariumStatsUI : MonoBehaviour
{
    [Header("Stats UI")]
    public TMP_Text carryingCapacityText;
    public TMP_Text oxygenText;
    public TMP_Text speedText;
    public TMP_Text pounceSpeedText;

    private void Start()
    {
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        PlayerStatsManager stats = PlayerStatsManager.Instance;

        carryingCapacityText.text = stats.carryingCapacity.ToString();

        oxygenText.text = stats.oxygenCapacity.ToString();

        speedText.text = stats.speed.ToString();

        pounceSpeedText.text = stats.pounceSpeed.ToString();
    }
}