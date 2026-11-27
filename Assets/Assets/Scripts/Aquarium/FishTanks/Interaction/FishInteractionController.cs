﻿using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Aquarium.FishTanks.Decor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Aquarium.FishTanks.Interaction
{
    [RequireComponent(typeof(AquariumCursorMapper))]
    [RequireComponent(typeof(FishDragHandler))]
    [RequireComponent(typeof(FishFeedingHandler))]
    [RequireComponent(typeof(TankCleaningHandler))]
    public class FishInteractionController : MonoBehaviour
    {
        public FishInteractionMode Mode { get; private set; } = FishInteractionMode.MoveFish;

        public readonly UnityEvent<TankFish, Vector2> DropFish = new();
        public readonly UnityEvent<PlacedTankDecorationObject> SelectTankDecor = new();
        public readonly UnityEvent<TankFish, Vector2> FeedFish = new();
        public readonly UnityEvent<Vector2> CleanAtPosition = new();
        
        public AquariumCursorMapper CursorMapper { get; private set; }
        private FishDragHandler _dragHandler;
        private FishFeedingHandler _feedingHandler;
        private TankCleaningHandler _cleaningHandler;

        private TankDecorInteractionHandler _tankDecorHandler;

        private void Awake()
        {
            CursorMapper = GetComponent<AquariumCursorMapper>();
            
            _dragHandler = GetComponent<FishDragHandler>();
            _dragHandler.OnDrop.AddListener(DropFish.Invoke);
            
            _feedingHandler = GetComponent<FishFeedingHandler>();
            _feedingHandler.FeedFish.AddListener(FeedFish.Invoke);
            
            _cleaningHandler = GetComponent<TankCleaningHandler>();
            _cleaningHandler.OnCleanPosition.AddListener(CleanAtPosition.Invoke);

            _tankDecorHandler = GetComponent<TankDecorInteractionHandler>();
            _tankDecorHandler.OnSelectingTankDecor.AddListener(SelectTankDecor.Invoke);
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            Vector2 mousePosition = mouse.position.ReadValue();

            _tankDecorHandler.UpdateHover(mousePosition);
            _dragHandler.UpdateHover(mousePosition);
            
            switch (Mode)
            {
                case FishInteractionMode.MoveFish:
                    _dragHandler.Tick(
                        mousePosition,
                        mouse.leftButton.wasPressedThisFrame,
                        mouse.leftButton.isPressed,
                        mouse.leftButton.wasReleasedThisFrame
                    );
                    _tankDecorHandler.Tick(
                        mousePosition,
                        mouse.leftButton.wasPressedThisFrame
                    );
                    break;
                case FishInteractionMode.CleanTank:
                    _cleaningHandler.Tick(
                        mousePosition,
                        mouse.leftButton.isPressed
                    );
                    break;
            }
        }
        
        public void SetMode(FishInteractionMode newMode)
        {
            if (Mode == newMode)
                return;
            
            _dragHandler.Drop();
            
            Mode = newMode;
        }

        private void OnEnable()
        {
            CursorMapper.SetRenderCameraActive(true);
        }

        private void OnDisable()
        {
            CursorMapper.SetRenderCameraActive(false);
            _dragHandler.Drop();
            Mode = FishInteractionMode.MoveFish;
        }


        public void BeginFeedDrag() => 
            SetMode(FishInteractionMode.FeedFish);

        public void EndFeedDrag(Vector2 screenPosition)
        {
            if (Mode != FishInteractionMode.FeedFish)
                return;

            _feedingHandler.TryFeed(screenPosition);

            SetMode(FishInteractionMode.MoveFish);
        }
    }
}