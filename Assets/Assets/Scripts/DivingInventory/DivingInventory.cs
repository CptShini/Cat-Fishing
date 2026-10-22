using System.Collections.Generic;
using UnityEngine;

public class DivingInventory : MonoBehaviour
{

    public static DivingInventory Instance { get; private set; }

    public List<Collectible> collectibles;

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
        if (IsFull())
        {
            Debug.Log("INVENTORY IS FULL");
            return false;
        }
        Debug.Log($"COLLECTIBLE: {collectible.collectibleName}");
        collectibles.Add(collectible);

        return true;
    }

    public void Remove(Collectible collectible)
    {
        collectibles.Remove(collectible);
    }

    bool IsFull()
    {
        return false; // TO BE ADDED
    }

}
