using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Diving;
using UnityEngine;

public class DivingInventory : MonoBehaviour
{
    [SerializeField] private DivingInventorySlot slotPrefab;
    [SerializeField] public GameObject panel;
    [SerializeField] private Transform grid;
    [SerializeField] private StatBar weightBar;

    public static DivingInventory Instance { get; private set; }
    public List<Collectible> collectibles;

    public float maxWeight;
    public bool isEnabled;
    private float curWeight;

    private void Awake()
    {
        collectibles = new();
        weightBar.SetMaxValue(maxWeight, curWeight);
        
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }

    public bool Add(Collectible collectible)
    {
        if (IsFull(collectible.weight))
        {
            print("INVENTORY IS FULL");
            return false;
        }
        
        collectibles.Add(collectible);
        
        SetWeight(collectible.weight);
        CreateSlot(collectible);

        return true;
    }

    private void SetWeight(float weight)
    {
        curWeight += weight;
        weightBar.CurrentValue = curWeight;
    }

    private void CreateSlot(Collectible collectible)
    {
        DivingInventorySlot slot = Instantiate(slotPrefab, grid, false);
        slot.Setup(collectible);
    }

    public void Drag(DivingInventorySlot inventorySlot)
    {
        Debug.Log("DRAGGING");
    }

    public void Toggle() => SetEnabled(!isEnabled);
    public void SetEnabled(bool enable)
    {
        isEnabled = enable;
        panel.SetActive(isEnabled);
    }

    public void Remove(Collectible collectible)
    {
        collectibles.Remove(collectible);
    }

    private bool IsFull(float weightToAdd) => 
        curWeight + weightToAdd > maxWeight;

    public List<T> GetCollectibleofType<T>() where T : Collectible => 
        collectibles.OfType<T>().ToList();
}
