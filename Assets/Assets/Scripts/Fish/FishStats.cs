using UnityEngine;

public class FishStats : MonoBehaviour
{
    [SerializeField] private FishSO fishSO;

    private Fish _fish;

    public Fish fish => _fish;

    void Awake()
    {
        _fish = new Fish(fishSO);
    }
}
