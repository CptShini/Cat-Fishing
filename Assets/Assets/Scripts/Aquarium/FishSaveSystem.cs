using System;
using System.Collections.Generic;
using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Domain;
using UnityEngine;

public static class FishSaveSystem
{
    private const string SaveKey = "AquariumFishSave";

    [Serializable]
    private class FishData
    {
        public string fishId;
        public int fishSOId;
        public string fishName;
        public float weight;
    }

    [Serializable]
    private class TankData
    {
        public string tankId;
        public List<FishData> fishes = new();
    }

    [Serializable]
    private class SaveData
    {
        public List<FishData> inventory = new();
        public List<TankData> tanks = new();
    }

    private static SaveData Load()
    {
        string json = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(json))
            return new SaveData();

        try
        {
            return JsonUtility.FromJson<SaveData>(json)
                   ?? new SaveData();
        }
        catch (Exception e)
        {
            Debug.LogError("Fish save load failed: " + e.Message);
            return new SaveData();
        }
    }

    private static void Write(SaveData data)
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private static FishData ToData(FishDTO fishDto)
    {
        return new FishData
        {
            fishId = fishDto.Id,
            fishSOId = fishDto.SpeciesId,
            fishName = fishDto.Name,
            weight = fishDto.Weight
        };
    }

    private static FishDTO FromData(FishData data, FishDatabase database)
    {
        FishSpecies type = database != null ? database.GetFishSO(data.fishSOId) : null;

        return new FishDTO
        {
            Id = data.fishId,
            SpeciesId = data.fishSOId,
            Name = data.fishName,
            Weight = data.weight,
            sprite = type != null ? type.Icon : null
        };
    }

    public static void SaveInventory(IReadOnlyList<FishDTO> fish)
    {
        SaveData data = Load();
        data.inventory.Clear();

        foreach (FishDTO item in fish)
        {
            if (item != null)
                data.inventory.Add(ToData(item));
        }

        Write(data);
    }

    public static List<FishDTO> LoadInventory(FishDatabase database)
    {
        SaveData data = Load();
        List<FishDTO> result = new();

        foreach (FishData item in data.inventory)
            result.Add(FromData(item, database));

        return result;
    }

    public static void SaveTank(FishTank tank)
    {
        if (tank == null || string.IsNullOrWhiteSpace(tank.TankId))
            return;

        SaveData data = Load();

        TankData entry = data.tanks.Find(t => t.tankId == tank.TankId);

        if (entry == null)
        {
            entry = new TankData { tankId = tank.TankId };
            data.tanks.Add(entry);
        }

        entry.fishes.Clear();

        foreach (FishDTO fish in tank.Fishes)
        {
            if (fish != null)
                entry.fishes.Add(ToData(fish));
        }

        Write(data);
    }

    public static List<FishDTO> LoadTank(string tankId, FishDatabase database)
    {
        SaveData data = Load();

        TankData entry = data.tanks.Find(t => t.tankId == tankId);

        List<FishDTO> result = new();

        if (entry == null)
            return result;

        foreach (FishData item in entry.fishes)
            result.Add(FromData(item, database));

        return result;
    }

    public static void ResetFishSave()
    {
        PlayerPrefs.DeleteKey("FishInventory");
        PlayerPrefs.DeleteKey("FishTanks");

        PlayerPrefs.Save();

        Debug.Log("Fish inventory and tank saves have been reset.");
    }
}