using System.Linq;
using Assets.Scripts.Fishies.CatchMinigame;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fishies.Movement
{
    public class FishCaughtMovement : MonoBehaviour
    {
        public Vector2 Direction { get; private set; }
        public readonly UnityEvent<Vector2> OnLose = new();

        private FishCatchPattern _catchPattern;
        private float _patternSpeed;
        
        public bool Catching { get; private set; }
        private float _catchClock;
        private CatchPatternMove _currentMove;

        public void Init(FishCatchPattern catchPattern, float patternSpeed)
        {
            _catchPattern = catchPattern;
            _patternSpeed = patternSpeed;
        }
        
        private void Update()
        {
            if (!Catching) return;
            
            _catchClock -= Time.deltaTime * _patternSpeed;
            if (_catchClock > 0f) return;
            
            NextMove(_currentMove.move);
        }

        public void Catch()
        {
            Catching = true;

            FishMove initialMove = _catchPattern.GetUsedMoves().GetRandom();
            NextMove(initialMove);
        }

        public void Lose(Vector2 playerPos)
        {
            Catching = false;
            _catchClock = 0f;
            
            OnLose.Invoke(playerPos);
        }

        private void NextMove(FishMove previousMove)
        {
            _currentMove = GenerateNextMove();
            _catchClock = _currentMove.duration;
            
            Direction = _currentMove.move.ToVector2();
            
            return;
            
            CatchPatternMove GenerateNextMove()
            {
                CatchPatternMove[] possibleNextMoves = _catchPattern.GetMoves(previousMove);
                CatchPatternMove nextMove = RandomHelper.WeightedRandom(possibleNextMoves
                    .ToDictionary(m => m, w => w.weight)
                );

                return nextMove;
            }
        }
    }
}