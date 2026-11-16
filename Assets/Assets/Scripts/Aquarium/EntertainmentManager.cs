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

        private int CurrentCrowd => Mathf.RoundToInt(_currentCrowdScore);
        private float 
            _currentCrowdScore,
            _targetCrowdScore;
        
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
            _targetCrowdScore = entertainmentSum * k;
        }

        public void DayPassed()
        {
            float crowdScoreDelta = _targetCrowdScore - _currentCrowdScore;
            _currentCrowdScore += crowdScoreDelta * crowdGrowthRate;
            
            int profit = CurrentCrowd * ticketPrice;
            GoldManager.Instance.AddGold(profit);
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
            if (data.DayPassed) UpdateCrowd();
        }

        public void SaveData(ref GameData data) => data.CrowdScore = _currentCrowdScore;
    }
}