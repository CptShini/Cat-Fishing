using Assets.Scripts.Aquarium.FishTanks;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Aquarium
{
    public class EntertainmentManager : MonoBehaviour, IDataPersistence
    {
        private float _decorationEntertainment;
        private FishTank[] _fishTanks;

        private void Start()
        {
            PlacementController.Instance.OnPlaced.AddListener(HandlePlacedFurniture);
            PlacementController.Instance.OnPickedUp.AddListener(HandlePickedUpFurniture);
        }

        private void HandlePlacedFurniture(ItemSO item) => 
            _decorationEntertainment += item.EntertainmentValue;

        private void HandlePickedUpFurniture(ItemSO item) => 
            _decorationEntertainment -= item.EntertainmentValue;

        public void LoadData(GameData data)
        {
            _decorationEntertainment = data.DecorationEntertainment;
            CrowdPopulator.Instance.InitializeCrowd(data.CrowdHistory[0]);
        }

        public void SaveData(ref GameData data)
        {
            data.DecorationEntertainment = _decorationEntertainment;
        }
    }
}