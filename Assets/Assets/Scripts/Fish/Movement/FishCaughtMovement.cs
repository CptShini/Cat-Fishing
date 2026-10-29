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
        public readonly UnityEvent<Vector2,bool> onLoose = new();

        private bool _catching;
        private float _catchClock;
        private CatchPatternMove _currentMove;
        private Vector2 _direction;

        public Vector2 direction => _direction;
                
        private void Update()
        {
            if (!_catching) return;
            
            if (_catchClock > 0f)
            {
                _catchClock -= Time.deltaTime * patternSpeed;
            }
            else
            {
                _catchClock = 0f;
                NextMove(_currentMove.move);
            }
        }

        public void Catch(FishSpeed speed)
        {
            if (_catching) return;
            
            _catching = true;
            
            FishMove initialMove = (FishMove)Random.Range(0, 4);
            NextMove(initialMove);
        }

        public void Caught()
        {
            if (!_catching) return;
            
            _catching = false;
            _catchClock = 0f;
        }

        public void Loose(Vector2 otherPos)
        {
            _catching = false;
            _catchClock = 0f;
            onLoose.Invoke(otherPos, true);
        }

        private void NextMove(FishMove previousMove)
        {
            _currentMove = GenerateNextMove(previousMove);
            _catchClock = _currentMove.duration;
            
            _direction = _currentMove.move.ToVector2();
        }

        private CatchPatternMove GenerateNextMove(FishMove previousMove)
        {
            CatchPatternMove[] possibleNextMoves = catchPattern.GetMoves(previousMove);
            CatchPatternMove nextMove = RandomHelper.WeightedRandom(possibleNextMoves
                .ToDictionary(m => m, w => w.weight)
            );

            return nextMove;
        }
    }
}