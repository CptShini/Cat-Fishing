using UnityEngine;

public class SaveResetButton : MonoBehaviour
{
    public void ResetSave()
    {
        FishTank[] tanks = FindObjectsByType<FishTank>(FindObjectsSortMode.None);

        foreach (FishTank tank in tanks)
        {
            if (tank != null)
                tank.ClearTank();
        }

        FishSaveSystem.ResetFishSave();

        if (FishCollectionManager.Instance != null)
        {
            FishCollectionManager.Instance.ReloadInventoryAfterReset();
        }

        Debug.Log("Fish save reset complete.");
    }
}
