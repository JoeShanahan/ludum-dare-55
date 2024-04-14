using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace LudumDare55
{
    public class PlayerAvatar : BoardActor
    {
        public BookData Book { get; private set; }
        
        private Vector3 _desiredDirection;
        private bool _isAi;



        public void EnableAI()
        {
            _isAi = true;
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
            if (_isAi == false) { return; }
            
            int ypos = Mathf.RoundToInt(transform.localPosition.y);

            if (ypos is 4 or 0)
            {
                int i = Random.Range(0, 4);
                TrySummon(Book.Summons[i]);
            }
            
            CollidingActor = null;
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

        
        private bool _isMovingUp = true;

        private void DetermineAIAction()
        {
            Vector3 newPos = transform.localPosition;
            newPos += _isMovingUp ? Vector3.up : Vector3.down;

            if (_board.IsSpaceValid(newPos) == false)
            {
                _isMovingUp = !_isMovingUp;
                newPos = transform.localPosition;
                newPos += _isMovingUp ? Vector3.up : Vector3.down;
            }

            _desiredDirection = newPos - transform.localPosition;
        }
        
        
        public override void CommitToAction()
        {
            // Probably could be simplified
            if (_isAi)
                DetermineAIAction();

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
