using Assets.Scripts.Diving.Bars;
using Assets.Scripts.Management;
using UnityEngine;

namespace Assets.Scripts.Diving.Managers
{
    public class DivingManager : MonoBehaviour
    {
        [SerializeField] private StatBar
            oxygenBar;
        
        [SerializeField] private Color dangerColor;
        [SerializeField] private Color defaultColor;
        [SerializeField] [Range(0f, 1f)] private float dangerThreshold;
        
        private bool _danger;


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
            if (PlayerState == Scripts.PlayerState.PlayerState.GRAB) return;
            
            oxygenBar.CurrentValue -= TimeManager.Instance.catchingTimeScale * Time.deltaTime;
            
            switch (_danger)
            {
                case false when oxygenBar.CurrentPercent < dangerThreshold:
                    SetDanger(true);
                    break;
                case true when oxygenBar.CurrentPercent > dangerThreshold:
                    SetDanger(false);
                    break;
            }
        }

        private void SetDanger(bool danger)
        {
            oxygenBar.foreground.color = danger ? dangerColor : defaultColor;
            _danger = danger;
        }

        private void NoOxygen()
        {
            if (_noOxygen || GameManager.Instance.Transitioning) return;
        
            _noOxygen = true;
            GameManager.Instance.ChangeScene(Scenes.DailyReport, "You ran out of oxygen. You have to go back without your stuff...");
        }
    }
}
