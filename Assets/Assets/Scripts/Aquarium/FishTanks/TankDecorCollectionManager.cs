using System;
using System.Collections.Generic;
using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class TankDecorCollectionManager : MonoBehaviour, IDataPersistence
    {
        public static TankDecorCollectionManager Instance { get; private set; }
    
        public readonly UnityEvent TankDecorCollectionChanged = new();

        public IReadOnlyDictionary<Guid, TankDecoration> CollectedTankDecor => _collectedTankDecor;
        private Dictionary<Guid, TankDecoration> _collectedTankDecor = new();    
        [SerializeField] private TankDecorationTypeDatabase tankDecorationTypeDatabase;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Add(TankDecoration tankDecoration)
        {
            _collectedTankDecor.Add(tankDecoration.Id, tankDecoration);
            TankDecorCollectionChanged.Invoke();
        }

        public void AddRange(IReadOnlyList<TankDecoration> tankDecorations)
        {
            if (tankDecorations.Count == 0)
                return;
            
            foreach (TankDecoration tankDecoration in tankDecorations)
            {
                _collectedTankDecor.Add(tankDecoration.Id, tankDecoration);
            }
            
            TankDecorCollectionChanged.Invoke();
        }

        public bool Remove(TankDecoration tankDecoration)
        {
            bool collectedTankDecor = _collectedTankDecor.ContainsKey(tankDecoration.Id);
            if (!collectedTankDecor) return false;
        
            _collectedTankDecor.Remove(tankDecoration.Id);
            TankDecorCollectionChanged.Invoke();

            return true;
        }
        
        
        public void LoadData(GameData data)
        {
            _collectedTankDecor = data.TankDecorations.ToDictionary(dto => new TankDecoration(dto, tankDecorationTypeDatabase));
            TankDecorCollectionChanged.Invoke();
        }

        public void SaveData(ref GameData data)
        {
            data.TankDecorations = _collectedTankDecor.ToSerializableDictionary(tf => new TankDecorationDTO(tf));
        }
    }
}