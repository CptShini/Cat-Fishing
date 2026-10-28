
using System.Collections.Generic;
using UnityEngine;

public class FishCollectionList : MonoBehaviour
{
    [SerializeField] private List<Fish> FishList = new();

    public void AddFish()
    {
        foreach (Fish fish in FishList)
        {
            if (fish == null)
                continue;

            FishCollectionManager.Instance.AddFish(fish);
        }

        Debug.Log("Fish list added to collection!");
    }
}
