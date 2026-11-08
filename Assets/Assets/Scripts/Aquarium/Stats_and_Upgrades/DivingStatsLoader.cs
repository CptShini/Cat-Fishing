using UnityEngine;

public class DivingStatsLoader : MonoBehaviour
{
    // loads player stats from PlayerStatsManager from aquarium scene and applies them to DivingPlayerStats in diving scene
    // I didn't want to change DivingPlayerStats too much because it is attached to the player prefab and also other scripts are using it
    private void Start()
    {
        if (PlayerStatsManager.Instance == null)
            return;

        GetComponent<DivingPlayerStats>().ApplyStats(PlayerStatsManager.Instance);
    }
}