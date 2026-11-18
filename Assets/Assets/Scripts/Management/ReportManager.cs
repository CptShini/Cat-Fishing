using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Management
{
    public class ReportManager : MonoBehaviour, IDataPersistence
    {
        [Header("TextFields")]
        [SerializeField] private TextMeshProUGUI tankEntertainmentText;
        [SerializeField] private TextMeshProUGUI sumEntertainmentText;


        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI addedGoldText;

        [SerializeField] private TextMeshProUGUI currentCrowdText;
        [SerializeField] private TextMeshProUGUI addedCrowdText;

        [Header("Crowd Calculation Variables")]
        [SerializeField] private int ticketPrice = 2;
        [SerializeField] private float k = 2f;
        [SerializeField] private float crowdGrowthRate = 0.2f;

        [Header("Other")]
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;
        [SerializeField] private FishReportSlot slotPrefab;
        [SerializeField] private GameObject grid;

        private float _entertainmentSum;
        
        private float _currentCrowdScore;
        private History _crowdHistory;

        private int _currentCrowd, _addedCrowd;
        private int _currentGold, _addedGold;

        private Dictionary<EntertainmentContributor, float> _entertainmentValues;
        private IEnumerable<Fish> _collectedFish;
        
        private void Start()
        {
            AddFishToGrid();
            Calculate();
            SetResults();
        }

        private void AddFishToGrid()
        {
            foreach (var fish in _collectedFish)
            {
                FishReportSlot slot = Instantiate(slotPrefab, grid.transform, false);
                slot.Setup(fish);
            }
        }

        private void Calculate()
        {
            _entertainmentSum = _entertainmentValues.Values.Sum();

            float targetCrowdScore = _entertainmentSum * k;
            float crowdScoreTargetDelta = targetCrowdScore - _currentCrowdScore;
            float crowdScoreChange = crowdScoreTargetDelta * crowdGrowthRate;

            _currentCrowdScore += crowdScoreChange;
            _currentCrowd = Mathf.RoundToInt(_currentCrowdScore);
            _addedCrowd = _crowdHistory.Add(_currentCrowd);

            _addedGold = _currentCrowd * ticketPrice;
            _currentGold += _addedGold;
        }

        private void SetResults()
        {
            tankEntertainmentText.text = $"{_entertainmentValues[EntertainmentContributor.Tank]:F1}";
            sumEntertainmentText.text = $"{_entertainmentSum:F1}";
            
            currentCrowdText.text = _currentCrowd.ToString();
            addedCrowdText.text = GetChangeText(_addedCrowd);
            addedCrowdText.color = GetChangeColor(_addedCrowd);
        
            currentGoldText.text = _currentGold.ToString();
            addedGoldText.text = GetChangeText(_addedGold);
            addedGoldText.color = GetChangeColor(_addedGold);

            return;

            string GetChangeText(int change) => 
                $"({(change > 0 ? "+" : "")}{change})";

            Color GetChangeColor(int change) =>
                change switch
                {
                    > 0 => Color.darkGreen,
                    < 0 => Color.red,
                    0 => Color.gray
                };
        }

        public void LoadData(GameData data)
        {
            _currentGold = data.Gold;
            
            _currentCrowdScore = data.CrowdScore;
            _crowdHistory = new(data.CrowdHistory);
            
            _entertainmentValues = data.EntertainmentValues;
            _collectedFish = data.Collection.Select(kvp => new Fish(kvp.Value.Fish, fishSpeciesDatabase));
        }

        public void SaveData(ref GameData data)
        {
            data.Gold = _currentGold;
            
            data.CrowdScore = _currentCrowdScore;
            data.CrowdHistory = _crowdHistory.ToArray();
        }
    }
}
