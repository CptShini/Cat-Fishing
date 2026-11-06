using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.DivingInventory
{
    [RequireComponent(typeof(Button))]
    public class DivingInventorySlot : MonoBehaviour
    {
        private Collectible _collectible;
        private Button _button;

        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
    
        private void Start()
        {
            _button = gameObject.GetComponent<Button>();
            _button.onClick.AddListener(delegate { DivingInventory.Instance.Drag(this); });
        
            text.text = $"{Mathf.RoundToInt(_collectible.Weight)} kg";
        }

        public void Setup(Collectible collectible)
        {
            _collectible = collectible;
            image.sprite = collectible.Sprite;
        }
    }
}