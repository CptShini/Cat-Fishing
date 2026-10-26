using System.Linq;
using Assets.Scripts.Fish.CatchMinigame;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Fish.Movement
{
    [RequireComponent(typeof(FishMover))]
    public class FishCaughtMovement : MonoBehaviour
    {
        [SerializeField] private FishCatchPatternSO catchPattern;
        [SerializeField] private float patternSpeed = 1f;

        private bool _catching;
        private float _catchClock;
        private CatchPatternMove _currentMove;
        
        private FishMover _mover;
        
        private void Awake() => _mover = GetComponent<FishMover>();

        private void Update()
        {
            if (!_catching) return;
            
            if (_catchClock > 0f)
                _catchClock -= Time.deltaTime * patternSpeed;
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
            _mover.SetSpeed(speed);
            
            FishMove initialMove = (FishMove)Random.Range(0, 4);
            NextMove(initialMove);
        }

        public void Caught()
        {
            if (!_catching) return;
            
            _catching = false;
            _catchClock = 0f;
        }

        private void NextMove(FishMove previousMove)
        {
            _currentMove = GenerateNextMove(previousMove);
            _catchClock = _currentMove.duration;
            
            _mover.DesiredDirection = _currentMove.move.ToVector2();
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