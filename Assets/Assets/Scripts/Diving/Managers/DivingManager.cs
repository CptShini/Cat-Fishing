using Assets.Scripts.Diving.Bars;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Diving.Managers
{
    public class DivingManager : MonoBehaviour
    {
        [SerializeField] private int aquariumSceneIndex = 1;
        
        private bool _noOxygen;
        
    
        private void Start()
        {
            OxygenBar.Instance.BarEmpty.AddListener(NoOxygen);
        }

        private void NoOxygen()
        {
            if (_noOxygen) return;
        
            _noOxygen = true;
            SceneTransition.Instance.LoadScene(aquariumSceneIndex);
        }
    }
}
