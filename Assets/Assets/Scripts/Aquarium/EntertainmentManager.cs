using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Aquarium.FishTanks;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Aquarium
{
    public class EntertainmentManager : MonoBehaviour, IDataPersistence
    {
        private Dictionary<EntertainmentContributor, float> _entertainmentValues;
        private FishTank[] _fishTanks;

        private void Start()
        {
            _fishTanks = 
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<FishTank>()
                .ToArray();

            foreach (FishTank fishTank in _fishTanks)
            {
                fishTank.TankChanged.AddListener(TanksChanged);
            }
            TanksChanged();
        }

        private void TanksChanged()
        {
            float entertainmentSum = _fishTanks.Sum(ft => ft.Tank.Entertainment);
            _entertainmentValues[EntertainmentContributor.Tank] = entertainmentSum;
        }

        public void LoadData(GameData data)
        {
            _entertainmentValues = data.EntertainmentValues;
            CrowdPopulator.Instance.InitializeCrowd(data.CrowdHistory[0]);
        }

        public void SaveData(ref GameData data)
        {
            data.EntertainmentValues[EntertainmentContributor.Tank] = _entertainmentValues[EntertainmentContributor.Tank];
        }
    }
}