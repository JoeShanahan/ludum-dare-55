using DG.Tweening;
using UnityEngine;

namespace LudumDare55
{
    public class AoeEffect : MonoBehaviour
    {
        public enum AnimMode { None, ScaleUp };

        [SerializeField] private AnimMode _animMode;
        [SerializeField] private float _lifetime = 0.3f;

        private void OnEnable()
        {
            Destroy(gameObject, _lifetime + 0.02f);

            if (_animMode == AnimMode.ScaleUp)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(1, _lifetime / 2).SetEase(Ease.OutExpo);
                transform.DOScale(0, _lifetime / 2).SetEase(Ease.InExpo).SetDelay(_lifetime / 2);
            }
        }
    }
}
