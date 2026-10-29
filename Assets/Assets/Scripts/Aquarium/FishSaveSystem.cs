using System;
using System.Collections.Generic;
using UnityEngine;

public static class FishSaveSystem
{
    private const string SaveKey = "AquariumFishSave";

    [Serializable]
    private class FishData
    {
        public string fishId;
        public string fishSOId;
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

    private static FishData ToData(Fish fish)
    {
        return new FishData
        {
            fishId = fish.fishId,
            fishSOId = fish.fishSOId,
            fishName = fish.fishName,
            weight = fish.weight
        };
    }

    private static Fish FromData(FishData data, FishDatabase database)
    {
        FishSO type = database != null ? database.GetFishSO(data.fishSOId) : null;

        return new Fish
        {
            fishId = data.fishId,
            fishSOId = data.fishSOId,
            fishName = data.fishName,
            weight = data.weight,
            sprite = type != null ? type.fishIcon : null
        };
    }

    public static void SaveInventory(IReadOnlyList<Fish> fish)
    {
        SaveData data = Load();
        data.inventory.Clear();

        foreach (Fish item in fish)
        {
            if (item != null)
                data.inventory.Add(ToData(item));
        }

        Write(data);
    }

    public static List<Fish> LoadInventory(FishDatabase database)
    {
        SaveData data = Load();
        List<Fish> result = new();

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

        foreach (Fish fish in tank.Fishes)
        {
            if (fish != null)
                entry.fishes.Add(ToData(fish));
        }

        Write(data);
    }

    public static List<Fish> LoadTank(string tankId, FishDatabase database)
    {
        SaveData data = Load();

        TankData entry = data.tanks.Find(t => t.tankId == tankId);

        List<Fish> result = new();

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