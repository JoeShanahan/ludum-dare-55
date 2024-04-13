using DG.Tweening;
using UnityEngine;

namespace LudumDare55
{
    public class BoardActor : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _renderer;
        
        public bool IsRight { get; private set; }
        
        public void SetSprite(Sprite sprite, bool isRight)
        {
            IsRight = isRight;
            _renderer.sprite = sprite;
            _renderer.flipX = isRight == false;
        }

        public void DoMove(Vector3 direction, float time)
        {
            Vector3 newPos = transform.localPosition + direction;
            newPos.x = Mathf.RoundToInt(newPos.x);
            newPos.y = Mathf.RoundToInt(newPos.y);
            newPos.z = transform.localPosition.z;
            
            transform.DOLocalMoveX(newPos.x, time).SetEase(Ease.Linear);
            transform.DOLocalMoveY(newPos.y, time).SetEase(Ease.Linear);

            transform.DOLocalMoveZ(newPos.z - 0.5f, time / 2f).SetEase(Ease.OutSine);
            transform.DOLocalMoveZ(newPos.z, time / 2f).SetDelay(time / 2f).SetEase(Ease.InSine);
        }
    }
}
