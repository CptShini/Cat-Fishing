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
        private bool _dayPassed;
        
        private void Start() => DayPassed();

        private void DayPassed()
        {
            if (_dayPassed) return;
            
            IEnumerable<IDayAware> dayAwares =
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IDayAware>();

            foreach (IDayAware dayAware in dayAwares)
            {
                dayAware.DayPassed();
            }

            _dayPassed = true;
        }

        public void DayOver() => _dayPassed = false;

        public void LoadData(GameData data) => _dayPassed = data.DayPassed;
        public void SaveData(ref GameData data) => data.DayPassed = _dayPassed;
    }
}