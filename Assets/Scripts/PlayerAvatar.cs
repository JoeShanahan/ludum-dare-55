using UnityEngine;
using DG.Tweening;

namespace LudumDare55
{
    public class PlayerAvatar : BoardActor
    {
        private Vector3 _desiredDirection;

        public void ResetDesiredDirection()
        {
            _desiredDirection = Vector3.zero;
        }

        public void SetDesiredDirection(Vector3 dir)
        {
            _desiredDirection = dir;
        }
        
        public override Vector3 GetMoveDirection()
        {
            return _desiredDirection;
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
    }
}
