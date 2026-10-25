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
        
        private float _catchClock;
        private CatchPatternMove _currentMove;
        
        private FishMover _mover;
        
        private void Start() => _mover = GetComponent<FishMover>();

        private void Update()
        {
            _catchClock += Time.deltaTime;
        }

        public void Caught()
        {
            // TODO
            print("Caught!");
            
            _catchClock = 0f;
            //_currentMove = GetNextMove(null);
        }

        private CatchPatternMove GetNextMove(FishMove previousMove)
        {
            CatchPatternMove[] moves = catchPattern.GetMoves(previousMove == null ? (FishMove)Random.Range(0, 4) : previousMove);
            return RandomHelper.WeightedRandom(moves.ToDictionary(m => m, w => w.weight));
        }
    }
}