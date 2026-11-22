using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Aquarium
{
    [DefaultExecutionOrder(100)]
    public class DayManager : MonoBehaviour, IDataPersistence
    {
        private bool _dayStarted;
        
        private void Start() => DayStarted();

        private void DayStarted()
        {
            if (_dayStarted) return;
            
            IEnumerable<IDayAware> dayAwares =
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IDayAware>();

            foreach (IDayAware dayAware in dayAwares)
            {
                dayAware.DayStarted();
            }

            _dayStarted = true;
        }

        public void LoadData(GameData data) => _dayStarted = data.DayStarted;
        public void SaveData(ref GameData data) => data.DayStarted = _dayStarted;
    }
}