using DG.Tweening;
using UnityEngine;

namespace LudumDare55
{
    public class BoardActor : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _renderer;

        public bool IsRight { get; private set; }
        
        public SummonAction NextAction { get; protected set; }
        public Vector2Int NextPosition { get; protected set; }
        public Vector2Int NextDirection { get; protected set; }
        
        // TODO this is dumb
        public Vector2Int GridPosition => Vector2Int.RoundToInt(transform.localPosition);
        
        public virtual void CommitToAction()
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
            if (NextAction == SummonAction.Move)
            {
                DoMove(NextPosition, time);
            }
            else if (NextAction == SummonAction.Wait)
            {
                SkipTurnAnimation(time);
            }
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
