using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace LudumDare55
{
    public class PlayerAvatar : BoardActor
    {
        public BookData Book { get; private set; }

        [SerializeField] private ActiveGameState _gameState;
        private Vector3 _desiredDirection;
        private AIController _aiController;
        private bool _isAi;

        public void EnableAI()
        {
            _isAi = true;
            _renderer.sprite = _gameState.Opponent.Sprite;
            _aiController = this.gameObject.AddComponent<AIController>();
            _aiController.Init(this, _board);     
        }

        public void SetDesiredDirection(Vector3 dir)
        {
            _desiredDirection = dir;
        }

        public bool TrySummon(SummonData data)
        {

            if (_board.IsSpaceTaken(SummonPosition)) 
            { 
                return false; 
            }
            
            _board.CreateNewSummon(this, data);
            return true;

        }

        public override void OnMoveComplete()
        {
            foreach (BoardActor a in _board._pageActors)
            {
                if (a.GridPosition == this.GridPosition)
                {
                    _board._pageActors.Remove(a);
                    Destroy(a.gameObject);
                    if (_isAi) { _aiController.PagePickup(); }
                    else { _board.GameController.GetNewCard(); }
                    break;
                }
                if (_isAi) { _aiController.AttemptSummon(); }
                CollidingActor = null;
            }
        }
        
        public Vector3 SummonPosition
        {
            get
            {
                if (IsRight)
                    return transform.localPosition + Vector3.right;

                return transform.localPosition + Vector3.left;
            }
        }
        
        public override void DoAction(float time)
        {
            base.DoAction(time);
            _desiredDirection = Vector3.zero;
        }
        
        // TODO this shouldn't live on the Avatar, but a separate player object somewhere
        public void SetBook(BookData book)
        {
            Book = book;
        }     
              
        public override void CommitToAction()
        {
            // Probably could be simplified
            if (_isAi)
                _desiredDirection = _aiController.DetermineAIAction();

            if (_desiredDirection == Vector3.zero)
            {
                NextAction = BoardAction.Wait;
                NextDirection = Vector2Int.zero;
                NextPosition = GridPosition;
            }
            else
            {
                NextAction = BoardAction.Move;
                NextDirection = Vector2Int.RoundToInt(_desiredDirection);
                NextPosition = GridPosition + NextDirection;
            }
        }
    }
}
