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
        [SerializeField] private float dangerThreshold; // between 0 and 1
        private bool danger;


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
                oxygenBar.CurrentValue -= TimeManager.Instance.catchingTimeScale * Time.deltaTime;
                if(!danger && oxygenBar.CurrentValue / oxygenBar.MaxVal < dangerThreshold)
                {
                    SetDanger(true);
                }
                else if(danger && oxygenBar.CurrentValue / oxygenBar.MaxVal > dangerThreshold)
                {
                    SetDanger(false);
                }
            }
        }

        private void SetDanger(bool danger)
        {
            oxygenBar.foreground.color = danger ? dangerColor : defaultColor;
            this.danger = danger;
        }

        private void NoOxygen()
        {
            if (_noOxygen || GameManager.Instance.Transitioning) return;
        
            _noOxygen = true;
            GameManager.Instance.ChangeScene(Scenes.DailyReport, "You ran out of oxygen. You have to go back without your fish...");
        }
    }
}
