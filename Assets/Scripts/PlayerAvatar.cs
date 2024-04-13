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
    }
}
