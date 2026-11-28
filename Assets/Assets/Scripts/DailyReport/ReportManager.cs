using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.DailyReport
{
    public class ReportManager : MonoBehaviour, IDataPersistence
    {
        [Header("TextFields")]
        [SerializeField] private TextMeshProUGUI tankEntertainmentText;
        [SerializeField] private TextMeshProUGUI decorEntertainmentText;
        [SerializeField] private TextMeshProUGUI decorPercentBonusText;
        [SerializeField] private TextMeshProUGUI sumEntertainmentText;
        
        [SerializeField] private TextMeshProUGUI currentGoldText, addedGoldText;
        [SerializeField] private TextMeshProUGUI currentCrowdText, addedCrowdText;

        [SerializeField] private TextMeshProUGUI
            hungerDeathText,
            oldAgeDeathText,
            dirtinessDeathText;

        [Header("Crowd Calculation Variables")]
        [SerializeField] private int ticketPrice = 2;
        [SerializeField] private float k = 2f;
        [SerializeField] private float crowdGrowthRate = 0.2f;

        [Header("Other")]
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;
        [SerializeField] private TankDecorationTypeDatabase tankDecorationTypeDatabase;

        [SerializeField] private CollectibleReportSlot slotPrefab;
        [SerializeField] private GameObject grid;

        private float _entertainmentSum;
        
        private float _currentCrowdScore;
        private History _crowdHistory;

        private int _currentCrowd, _addedCrowd;
        private int _currentGold, _addedGold;

        private float _tankEntertainment;
        private int _decorationEntertainmentPercentBonus;
        private IEnumerable<Fish> _collectedFish;
        private Dictionary<Guid, Tank> _tanks;
        private IEnumerable<TankDecoration> _collectedTankDecorations;
        
        private int[] _fishDeathCount;
        
        private void Start()
        {
            AddCollectibleToGrid();
            DayEnded();
            Calculate();
            SetResults();
        }

        private void AddCollectibleToGrid()
        {
            foreach (var fish in _collectedFish)
            {
                CollectibleReportSlot slot = Instantiate(slotPrefab, grid.transform, false);
                slot.Setup(fish.Species.Name, fish.Species.Icon);
            }

            foreach (var tankDecor in _collectedTankDecorations)
            {
                CollectibleReportSlot slot = Instantiate(slotPrefab, grid.transform, false);
                slot.Setup(tankDecor.Type.Name, tankDecor.Type.Icon);
            }
        }

        private void DayEnded()
        {
            _fishDeathCount = new int[3];
            foreach (Tank tank in _tanks.Values)
            {
                tank.DayEnded();
                _tankEntertainment += tank.Entertainment;

                for (int i = 0; i < 3; i++)
                {
                    _fishDeathCount[i] += tank.FishDeathCount[i];
                }
            }
        }

        private void Calculate()
        {
            float decorationEntertainmentBonus = 1f + _decorationEntertainmentPercentBonus / 100f;
            _entertainmentSum = _tankEntertainment * decorationEntertainmentBonus;

            float targetCrowdScore = Mathf.Sqrt(_entertainmentSum) * k;
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
            tankEntertainmentText.text = $"{_tankEntertainment:F1}";
            decorPercentBonusText.text = $"({_decorationEntertainmentPercentBonus}%)";
            
            float decorationContribution = _entertainmentSum - _tankEntertainment;
            decorEntertainmentText.text = $"{decorationContribution:F1}";
            sumEntertainmentText.text = $"{_entertainmentSum:F1}";
            
            currentCrowdText.text = _currentCrowd.ToString();
            addedCrowdText.text = GetChangeText(_addedCrowd);
            addedCrowdText.color = GetChangeColor(_addedCrowd);
        
            currentGoldText.text = _currentGold.ToString();
            addedGoldText.text = GetChangeText(_addedGold);
            addedGoldText.color = GetChangeColor(_addedGold);

            hungerDeathText.text = _fishDeathCount[(int)FishCauseOfDeath.Hunger].ToString();
            oldAgeDeathText.text = _fishDeathCount[(int)FishCauseOfDeath.OldAge].ToString();
            dirtinessDeathText.text = _fishDeathCount[(int)FishCauseOfDeath.Dirtiness].ToString();

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
            
            _decorationEntertainmentPercentBonus = data.DecorationEntertainmentPercentBonus;
            
            _collectedFish = data.Collection.Select(kvp => new Fish(kvp.Value.Fish, fishSpeciesDatabase));
            _tanks = data.Tanks.ToDictionary(dto => new Tank(dto, fishSpeciesDatabase, tankDecorationTypeDatabase));
            _collectedTankDecorations = data.DailyTankDecorations.Select(kvp => new TankDecoration(kvp.Value, tankDecorationTypeDatabase));
        }

        public void SaveData(ref GameData data)
        {
            if (!data.DayStarted)
                return;
                
            data.DayStarted = false;
            data.Gold = _currentGold;
            
            data.CrowdScore = _currentCrowdScore;
            data.CrowdHistory = _crowdHistory.ToArray();

            data.DailyTankDecorations.Clear();
            data.Tanks = _tanks.ToSerializableDictionary(tank => new TankDTO(tank));
        }
    }
}
