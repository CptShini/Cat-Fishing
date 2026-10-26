using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class DivingInventory : MonoBehaviour
{
    [SerializeField] private DivingInventorySlot slotPrefab;
    [SerializeField] public GameObject panel;
    [SerializeField] private StatBar weightBar;

    public static DivingInventory Instance { get; private set; }
    public List<Collectible> collectibles;

    public float maxWeight;
    public bool isEnabled = false;
    private float curWeight;

    private void Awake()
    {
        collectibles = new List<Collectible>();
        weightBar.SetMaxValue(maxWeight);
        weightBar.SetCurrentValue(curWeight);
        
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }
    void Start()
    {
        panel.SetActive(isEnabled);
    }

    public bool Add(Collectible collectible)
    {
        if (IsFull(collectible.weight))
        {
            Debug.Log("INVENTORY IS FULL");
            return false;
        }
        Debug.Log($"COLLECTIBLE: {collectible.collectibleName}");
        collectibles.Add(collectible);
        
        SetWeight(collectible.weight);

        CreateSlot(collectible);

        return true;
    }

    private void SetWeight(float weight)
    {
        curWeight += weight;
        weightBar.SetCurrentValue(curWeight);
    }

    private void CreateSlot(Collectible collectible)
    {
        
        DivingInventorySlot slot = Instantiate(slotPrefab);
        slot.transform.SetParent(this.transform, false);
        slot.Setup(collectible);
    }

    public void Drag(DivingInventorySlot inventorySlot)
    {
        Debug.Log("DRAGGING");
        
    }

    public void Toggle()
    {
        this.isEnabled = !this.isEnabled;
        panel.SetActive(this.isEnabled);
    }

    public void SetEnabled(bool enabled)
    {
        this.isEnabled = enabled;
        panel.SetActive(this.isEnabled);
    }

    public void Remove(Collectible collectible)
    {
        collectibles.Remove(collectible);
    }

    bool IsFull(float weightToAdd)
    {
        if((curWeight + weightToAdd) > maxWeight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
