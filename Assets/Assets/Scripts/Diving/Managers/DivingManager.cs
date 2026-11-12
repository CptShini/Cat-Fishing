using Assets.Scripts.Diving.Bars;
using Assets.Scripts.Management;
using UnityEngine;

namespace Assets.Scripts.Diving.Managers
{
    public class DivingManager : MonoBehaviour
    {
        [SerializeField] private StatBar
            oxygenBar;

        private static PlayerState.PlayerState PlayerState => DivingController.Instance.state;
        private static DivingPlayerStats PlayerStats => DivingPlayerStats.Instance;
        
        private bool _noOxygen;
        
        private void Start()
        {
            oxygenBar.SetMaxValue(PlayerStats.oxygenCapacity);
            oxygenBar.BarEmpty.AddListener(NoOxygen);
        }

        private void Update()
        {
            if (PlayerState != Scripts.PlayerState.PlayerState.GRAB)
            {
                oxygenBar.CurrentValue -= Time.deltaTime;
            }
        }

        private void NoOxygen()
        {
            if (_noOxygen) return;
        
            _noOxygen = true;
            GameManager.Instance.ChangeScene(Scenes.Aquarium);
        }
    }
}
