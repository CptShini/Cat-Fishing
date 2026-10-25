using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class DivingInventory : MonoBehaviour
{
    [SerializeField] private DivingInventorySlot slotPrefab;
    public static DivingInventory Instance { get; private set; }

    

    public List<Collectible> collectibles;

    public float maxWeight;
    private float curWeight;

    private void Awake()
    {
        if (collectibles == null)
        {
            collectibles = new List<Collectible>();
        }

        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
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

        DivingInventorySlot slot = Instantiate(slotPrefab);
        slot.transform.SetParent(this.transform, false);
        slot.Setup(collectible);

        return true;
    }

    public void Drag(DivingInventorySlot inventorySlot)
    {
        Debug.Log("DRAGGING");
        
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
