using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fish.Domain;
using UnityEngine;

public static class FishSaveSystem
{
    private const string SaveKey = "AquariumFishSave";

    [Serializable]
    private class TankData
    {
        public string tankId;
        public List<FishDTO> fishes = new();
    }

    [Serializable]
    private class SaveData
    {
        public List<FishDTO> inventory = new();
        public List<TankData> tanks = new();
    }

    private static SaveData Load()
    {
        string json = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(json))
            return new();

        try
        {
            return JsonUtility.FromJson<SaveData>(json)
                   ?? new SaveData();
        }
        catch (Exception e)
        {
            Debug.LogError("Fish save load failed: " + e.Message);
            return new();
        }
    }

    private static void Write(SaveData data)
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static void SaveInventory(IReadOnlyList<Fish> fish)
    {
        SaveData data = Load();
        data.inventory.Clear();

        foreach (Fish item in fish)
        {
            if (item != null)
                data.inventory.Add(new(item));
        }

        Write(data);
    }

    public static List<Fish> LoadInventory(FishSpeciesDatabase db)
    {
        SaveData data = Load();
        return data.inventory
            .Select(fishDTO => new Fish(fishDTO, db))
            .ToList();
    }

    public static void SaveTank(FishTank tank)
    {
        if (tank == null || string.IsNullOrWhiteSpace(tank.TankId))
            return;

        SaveData data = Load();

        TankData entry = data.tanks.Find(t => t.tankId == tank.TankId);

        if (entry == null)
        {
            entry = new() { tankId = tank.TankId };
            data.tanks.Add(entry);
        }

        entry.fishes.Clear();

        foreach (Fish fish in tank.Fishes)
        {
            if (fish != null)
                entry.fishes.Add(new(fish));
        }

        Write(data);
    }

    public static List<Fish> LoadTank(string tankId, FishSpeciesDatabase db)
    {
        SaveData data = Load();

        TankData entry = data.tanks.Find(t => t.tankId == tankId);

        List<Fish> result = new();

        if (entry == null)
            return result;

        result.AddRange(entry.fishes.Select(fishDTO => new Fish(fishDTO, db)));

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