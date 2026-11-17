using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;

public class ReportManager : MonoBehaviour, IDataPersistence
{
    [Header("TextFields")]
    [SerializeField] private TextMeshProUGUI tankEntertainmentText;
    [SerializeField] private TextMeshProUGUI sumEntertainmentText;


    [SerializeField] private TextMeshProUGUI currentGoldText;
    [SerializeField] private TextMeshProUGUI addedGoldText;

    [SerializeField] private TextMeshProUGUI currentCrowdText;
    [SerializeField] private TextMeshProUGUI addedCrowdText;

    [Header("CrowdCalulation Variables")]
    [SerializeField] private int ticketPrice = 2;
    [SerializeField] private float k = 2f;
    [SerializeField] private float crowdGrowthRate = 0.2f;

    [Header("Other")]
    [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;
    [SerializeField] private FishReportSlot slotPrefab;
    [SerializeField] private GameObject grid;

    private float _curCrowd = 0;
    private float _addedCrowd = 0;
    private int _curGold = 0;
    private int _addedGold = 0;

    private Dictionary<EntertainmentContributor, float> _entertainmentValues;
    private IReadOnlyDictionary<Guid, TankFish> _collectedFish;

    private float _entertainmentSum = 0;
    private int CurrentCrowd => Mathf.RoundToInt(_curCrowd);
    private int SumCrowd => Mathf.RoundToInt(_curCrowd + _addedCrowd);
    private int AddedCrowd => SumCrowd - CurrentCrowd;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tankEntertainmentText.text = $"{_entertainmentValues[EntertainmentContributor.Tank]}";
        AddFishToGrid();
        Calculate();
    }

    private void AddFishToGrid()
    {
        foreach (var fish in _collectedFish.Values)
        {
            CreateSlot(fish.Fish);
        }
    }

    private void CreateSlot(Fish fish)
    {
        FishReportSlot slot = Instantiate(slotPrefab, grid.transform, false);
        slot.Setup(fish);
    }

    private void Calculate()
    {
        
        SetEnterTainmentSum();

        float _targetCrowdScore = _entertainmentSum * k;
        float crowdScoreDelta = _targetCrowdScore - _curCrowd;
        _addedCrowd = crowdScoreDelta * crowdGrowthRate;
        
        _addedGold = SumCrowd * ticketPrice;

        SetResults();
    }

    private void SetEnterTainmentSum()
    {
        
        foreach (var entertainmentValue in _entertainmentValues.Values)
        {
            _entertainmentSum += entertainmentValue;
        }

        sumEntertainmentText.text = $"{_entertainmentSum}";
    }

    private void SetResults()
    {
        if(AddedCrowd > 0)
        {
            addedCrowdText.color = Color.darkGreen;
            addedCrowdText.text = $"(+{AddedCrowd})";
        }
        else if(AddedCrowd < 0)
        {
            addedCrowdText.color = Color.red;
            addedCrowdText.text = $"(-{Math.Abs(AddedCrowd)})";
        }
        else
        {
            addedCrowdText.color = Color.gray;
            addedCrowdText.text = $"({Math.Abs(AddedCrowd)})";
        }


        if(_addedGold > 0)
        {
            addedGoldText.color = Color.darkGreen;
            addedGoldText.text = $"(+{_addedGold})";
        }
        else if(_addedGold < 0)
        {
            addedGoldText.color = Color.red;
            addedGoldText.text = $"(-{Math.Abs(_addedGold)})";
        }
        else
        {
            addedGoldText.color = Color.gray;
            addedGoldText.text = $"({Math.Abs(_addedGold)})";
        }

        currentGoldText.text = $"{_curGold + _addedGold}";
        currentCrowdText.text = $"{SumCrowd}";
    }

    public void LoadData(GameData data)
    {
        _curGold = data.Gold;
        _curCrowd = data.CrowdScore;
        _entertainmentValues = data.EntertainmentValues;
        _collectedFish = data.Collection.ToDictionary(dto => new TankFish(dto, fishSpeciesDatabase));
    }

    public void SaveData(ref GameData data)
    {
        data.Gold = _curGold + _addedGold;
        data.CrowdScore = _curCrowd + _addedCrowd;
    }
}
