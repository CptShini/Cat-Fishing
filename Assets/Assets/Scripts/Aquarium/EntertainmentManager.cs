using Assets.Scripts.Aquarium.FishTanks;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Aquarium
{
    public class EntertainmentManager : MonoBehaviour, IDataPersistence
    {
        private int _decorationEntertainmentPercentBonus;
        private FishTank[] _fishTanks;

        private void Start()
        {
            PlacementController.Instance.OnPlaced.AddListener(HandlePlacedFurniture);
            PlacementController.Instance.OnPickedUp.AddListener(HandlePickedUpFurniture);
        }

        private void HandlePlacedFurniture(ItemSO item) => 
            _decorationEntertainmentPercentBonus += item.EntertainmentPercentBonus;

        private void HandlePickedUpFurniture(ItemSO item) => 
            _decorationEntertainmentPercentBonus -= item.EntertainmentPercentBonus;

        public void LoadData(GameData data)
        {
            _decorationEntertainmentPercentBonus = data.DecorationEntertainmentPercentBonus;
            CrowdPopulator.Instance.InitializeCrowd(data.CrowdHistory[0]);
        }

        public void SaveData(ref GameData data)
        {
            data.DecorationEntertainmentPercentBonus = _decorationEntertainmentPercentBonus;
        }
    }
}