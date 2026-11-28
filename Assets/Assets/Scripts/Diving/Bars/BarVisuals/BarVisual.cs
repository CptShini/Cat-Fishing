using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars.BarVisuals
{
    public abstract class BarVisual : MonoBehaviour
    {
        [SerializeField] protected Image Image;
        
        public abstract void UpdateVisual(float p);
    }
}