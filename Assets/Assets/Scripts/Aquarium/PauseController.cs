using UnityEngine;

public class PauseController : MonoBehaviour
{
    private static PauseController _instance;
    private bool _paused;

    public static bool IsGamePaused
    {
        get => _instance._paused;
        set => _instance._paused = value;
    }

    private void Awake() => _instance = this;
    private void Start() => _paused = false;
}
