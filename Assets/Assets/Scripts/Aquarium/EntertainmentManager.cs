using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Aquarium.FishTanks;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Aquarium
{
    public class EntertainmentManager : MonoBehaviour, IDataPersistence, IDayAware
    {
        [SerializeField] private TextMeshProUGUI crowdText;
        
        private Dictionary<EntertainmentContributor, float> entertainmentValues;
        private FishTank[] _fishTanks;

        private int CurrentCrowd => Mathf.RoundToInt(_currentCrowdScore);
        private float _currentCrowdScore;


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
            entertainmentValues[EntertainmentContributor.Tank] = entertainmentSum;
        }

        public void DayPassed()
        {
            UpdateCrowd();
        }

        private void UpdateCrowd()
        {
            crowdText.text = $"{CurrentCrowd}";
            CrowdPopulator.Instance.InitializeCrowd(CurrentCrowd);
        }

        public void LoadData(GameData data)
        {
            _currentCrowdScore = data.CrowdScore;
            entertainmentValues = data.EntertainmentValues;
            if (data.DayPassed) UpdateCrowd();
        }

        public void SaveData(ref GameData data)
        {
            data.EntertainmentValues[EntertainmentContributor.Tank] = entertainmentValues[EntertainmentContributor.Tank];
        }
    }
}