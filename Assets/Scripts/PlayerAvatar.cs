using System.Numerics;
using DG.Tweening;
using Vector3 = UnityEngine.Vector3;

namespace LudumDare55
{
    public class PlayerAvatar : BoardActor
    {
        public BookData Book { get; private set; }
        
        private Vector3 _desiredDirection;
        private bool _isAi;
        private BoardController _board;
        
        public void EnableAI()
        {
            _isAi = true;
        }

        public void Init(BoardController board)
        {
            _board = board;
        }
        
        public void SetDesiredDirection(Vector3 dir)
        {
            _desiredDirection = dir;
        }

        public void TrySummon()
        {
            if (_board.IsSpaceTaken(SummonPosition))
                return;
            
            _board.CreateNewSummon(this, Book.Summons[0]);
        }

        public void OnMoveComplete()
        {
            TrySummon();
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
            if (_isAi)
                DetermineAIAction();
            
            if (_desiredDirection.magnitude > 0)
            {
                DoMove(_desiredDirection, time);
                _desiredDirection = Vector3.zero;
            }
            else
            {
                SkipTurnAnimation(time);
            }
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
        
        private void SkipTurnAnimation(float time)
        {
            transform.DOScale(new Vector3(1.2f, 0.8f, 1f), time / 2f).SetEase(Ease.OutSine);
            transform.DOScale(Vector3.one, time / 2f).SetDelay(time / 2f).SetEase(Ease.InSine);
        }
    }
}
