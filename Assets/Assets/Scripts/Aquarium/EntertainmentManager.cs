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
        [SerializeField] private int ticketPrice = 2;
        [SerializeField] private float k = 2f;
        [SerializeField] private float crowdGrowthRate = 0.2f;
        [SerializeField] private TextMeshProUGUI crowdText;
        
        private FishTank[] _fishTanks;

        private int 
            _currentCrowd,
            _targetCrowd;
        
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
            _targetCrowd = Mathf.RoundToInt(entertainmentSum * k);
        }

        public void DayPassed()
        {
            int crowdDelta = _targetCrowd - _currentCrowd;
            int change = crowdDelta >= 0
                ? Mathf.CeilToInt(crowdDelta * crowdGrowthRate)
                : Mathf.FloorToInt(crowdDelta * crowdGrowthRate);
            _currentCrowd += change;
            
            int profit = _currentCrowd * ticketPrice;
            GoldManager.Instance.AddGold(profit);
            UpdateCrowd();
        }

        private void UpdateCrowd()
        {
            crowdText.text = $"{_currentCrowd}";
            CrowdPopulator.Instance.InitializeCrowd(_currentCrowd);
        }

        public void LoadData(GameData data)
        {
            _currentCrowd = data.CurrentCrowd;
            if (data.DayPassed) UpdateCrowd();
        }

        public void SaveData(ref GameData data) => data.CurrentCrowd = _currentCrowd;
    }
}