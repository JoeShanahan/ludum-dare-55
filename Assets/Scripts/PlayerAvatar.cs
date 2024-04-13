using UnityEngine;
using DG.Tweening;

namespace LudumDare55
{
    public class PlayerAvatar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        
        InputSystem_Actions _input;
        private bool _isPlayerControlled;

        private void Start()
        {
        }
        
        public void SetSprite(Sprite sprite, bool isRight)
        {
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
