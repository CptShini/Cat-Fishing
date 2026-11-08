using UnityEngine;

public class AquariumStatsUpgrades : MonoBehaviour
{
    public AquariumStatsUI statsUI;

    public void UpgradeOxygen()
    {
        PlayerStatsManager.Instance.oxygenCapacity += 20;

        statsUI.UpdateStatsUI();
    }

    public void UpgradeSpeed()
    {
        PlayerStatsManager.Instance.speed += 2;

        statsUI.UpdateStatsUI();
    }

    public void UpgradeCarryingCapacity()
    {
        PlayerStatsManager.Instance.carryingCapacity += 5;

        statsUI.UpdateStatsUI();
    }

    public void UpgradePounceSpeed()
    {
        PlayerStatsManager.Instance.pounceSpeed += 5;

        statsUI.UpdateStatsUI();
    }
}