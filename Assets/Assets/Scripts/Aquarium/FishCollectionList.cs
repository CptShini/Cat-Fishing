
using System.Collections.Generic;
using UnityEngine;

public class FishCollectionList : MonoBehaviour
{
    [SerializeField] private List<FishSO> FishList = new();

    public void AddFish()
    {
        foreach (FishSO fish in FishList)
        {
            if (fish == null)
                continue;

            FishCollectionManager.Instance.AddFish(fish);
        }

        Debug.Log("Fish list added to collection!");
    }
}
