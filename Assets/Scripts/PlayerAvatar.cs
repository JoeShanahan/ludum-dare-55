using UnityEngine;
using DG.Tweening;

namespace LudumDare55
{
    public class PlayerAvatar : BoardActor
    {
        private Vector3 _desiredDirection;

        public void SetDesiredDirection(Vector3 dir)
        {
            _desiredDirection = dir;
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
        
        private void SkipTurnAnimation(float time)
        {
            transform.DOScale(new Vector3(1.2f, 0.8f, 1f), time / 2f).SetEase(Ease.OutSine);
            transform.DOScale(Vector3.one, time / 2f).SetDelay(time / 2f).SetEase(Ease.InSine);
        }
    }
}
