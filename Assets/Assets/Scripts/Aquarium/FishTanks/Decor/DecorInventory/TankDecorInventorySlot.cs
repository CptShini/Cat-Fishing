using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain.TankDecor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTanks.DecorInventory
{
    public class TankDecorInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image tankDecorIcon;
        [SerializeField] private TMP_Text tankDecorName;

        private CanvasGroup canvasGroup;

        private TankDecoration _tankDecor;
        public static TankDecoration DraggedTankDecor { get; private set; }

        public readonly UnityEvent<TankDecorInventorySlot> OnClicked = new();
        public TankDecoration TankDecor => _tankDecor;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void SetTankDecor(TankDecoration tankDecoration)
        {
            if (tankDecoration == null) return;

            _tankDecor = tankDecoration;
            tankDecorIcon.sprite = tankDecoration.Type.Icon;
            tankDecorName.text = $"{tankDecoration.Type.Name}";
            tankDecorIcon.enabled = tankDecoration.Type.Icon != null;
        }

        public void OnClick()
        {
            OnClicked.Invoke(this);
        }

    }

}