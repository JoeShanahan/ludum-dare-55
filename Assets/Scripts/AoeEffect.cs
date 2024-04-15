using DG.Tweening;
using UnityEngine;

namespace LudumDare55
{
    public class AoeEffect : MonoBehaviour
    {
        public enum AnimMode { None, ScaleUp, VerticalScale };

        [SerializeField] private AnimMode _animMode;
        [SerializeField] private float _lifetime = 0.3f;
        [SerializeField] private bool _randomRot;

        private void OnEnable()
        {
            Destroy(gameObject, _lifetime + 0.02f);

            if (_animMode == AnimMode.ScaleUp)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(1, _lifetime / 2).SetEase(Ease.OutExpo);
                transform.DOScale(0, _lifetime / 2).SetEase(Ease.InExpo).SetDelay(_lifetime / 2);
            }
            if (_animMode == AnimMode.VerticalScale)
            {
                transform.localScale = new Vector3(1, 0, 1);
                transform.DOScaleY(1, _lifetime * 0.3f).SetEase(Ease.OutExpo);
                transform.DOScaleY(0, _lifetime * 0.7f).SetEase(Ease.InSine).SetDelay(_lifetime * 0.3f);
            }

            if (_randomRot)
            {
                transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            }
        }
    }
}
