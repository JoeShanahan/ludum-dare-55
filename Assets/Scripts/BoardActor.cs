using DG.Tweening;
using UnityEngine;

namespace LudumDare55
{
    public class BoardActor : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _renderer;

        public bool IsRight { get; private set; }
        public bool IsPage;

        public bool IsPlayer => IsRight;
        
        public BoardAction NextAction { get; protected set; }
        public Vector2Int NextPosition { get; protected set; }
        public Vector2Int NextDirection { get; protected set; }

        protected BoardController _board;
        
        // TODO this is dumb
        public Vector2Int GridPosition => Vector2Int.RoundToInt(transform.localPosition);
        
        public void Init(BoardController board)
        {
            _board = board;
        }

        
        public virtual void CommitToAction()
        {
            
        }

        public virtual void OnMoveComplete()
        {
            
        }

        public void SetSprite(Sprite sprite, bool isRight)
        {
            IsRight = isRight;
            _renderer.sprite = sprite;
            _renderer.flipX = isRight == false;
        }

        public virtual void DoAction(float time)
        {
            if (NextAction == BoardAction.Move)
            {
                DoMove(NextPosition, time);
            }
            else if (NextAction == BoardAction.Wait)
            {
                SkipTurnAnimation(time);
            }
            else if (NextAction == BoardAction.Bounce)
            {
                DoBounce(NextDirection, time, true);
            }
            else if (NextAction == BoardAction.HalfBounce)
            {
                DoBounce(NextDirection, time, false);
            }
        }

        public void OverrideNextAction(BoardAction action, Vector2Int endPosition)
        {
            NextAction = action;
            NextPosition = endPosition;
        }
        
        protected void DoBounce(Vector2Int direction, float time, bool isFull)
        {
            Vector3 newPos = transform.localPosition;
            float mul = isFull ? 0.7f : 0.35f;
            newPos += new Vector3(direction.x * mul, direction.y * mul, 0);

            float halfTime = time / 2f;

            transform.DOLocalMoveX(newPos.x, halfTime).SetEase(Ease.Linear);
            transform.DOLocalMoveY(newPos.y, halfTime).SetEase(Ease.Linear);
            
            transform.DOLocalMoveX(NextPosition.x, halfTime).SetDelay(halfTime).SetEase(Ease.Linear);
            transform.DOLocalMoveY(NextPosition.y, halfTime).SetDelay(halfTime).SetEase(Ease.Linear);

            transform.DOLocalMoveZ(newPos.z - 0.5f, halfTime).SetEase(Ease.OutSine);
            transform.DOLocalMoveZ(newPos.z, halfTime).SetDelay(halfTime).SetEase(Ease.InSine);
        }

        protected void DoMove(Vector2Int localPos, float time)
        {
            Vector3 newPos = new(localPos.x, localPos.y, transform.localPosition.z);

            transform.DOLocalMoveX(newPos.x, time).SetEase(Ease.Linear);
            transform.DOLocalMoveY(newPos.y, time).SetEase(Ease.Linear);

            transform.DOLocalMoveZ(newPos.z - 0.5f, time / 2f).SetEase(Ease.OutSine);
            transform.DOLocalMoveZ(newPos.z, time / 2f).SetDelay(time / 2f).SetEase(Ease.InSine);
        }
        
        protected void SkipTurnAnimation(float time)
        {
            transform.DOScale(new Vector3(1.2f, 0.8f, 1f), time / 2f).SetEase(Ease.OutSine);
            transform.DOScale(Vector3.one, time / 2f).SetDelay(time / 2f).SetEase(Ease.InSine);
        }
    }
}
