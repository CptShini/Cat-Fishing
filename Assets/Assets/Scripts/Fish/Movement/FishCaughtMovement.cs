using System.Linq;
using Assets.Scripts.Fish.CatchMinigame;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fish.Movement
{
    public class FishCaughtMovement : MonoBehaviour
    {
        [SerializeField] private FishCatchPatternSO catchPattern;
        [SerializeField] private float patternSpeed = 1f;
        
        public Vector2 Direction { get; private set; }
        public readonly UnityEvent<Vector2> OnLose = new();

        private bool _catching;
        private float _catchClock;
        private CatchPatternMove _currentMove;
                
        private void Update()
        {
            if (!_catching) return;
            
            if (_catchClock > 0f)
            {
                _catchClock -= Time.deltaTime * patternSpeed;
                return;
            }

            _catchClock = 0f;
            NextMove(_currentMove.move);
        }

        public void Catch()
        {
            if (_catching) return;
            
            _catching = true;

            FishMove initialMove = catchPattern.GetUsedMoves().GetRandom();
            NextMove(initialMove);
        }

        public void Lose(Vector2 otherPos)
        {
            _catching = false;
            _catchClock = 0f;
            OnLose.Invoke(otherPos);
        }

        private void NextMove(FishMove previousMove)
        {
            _currentMove = GenerateNextMove();
            _catchClock = _currentMove.duration;
            
            Direction = _currentMove.move.ToVector2();
            
            return;
            
            CatchPatternMove GenerateNextMove()
            {
                CatchPatternMove[] possibleNextMoves = catchPattern.GetMoves(previousMove);
                CatchPatternMove nextMove = RandomHelper.WeightedRandom(possibleNextMoves
                    .ToDictionary(m => m, w => w.weight)
                );

                return nextMove;
            }
        }

        
    }
}