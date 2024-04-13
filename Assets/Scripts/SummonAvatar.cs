using UnityEngine;

namespace LudumDare55
{
    public class SummonAvatar : BoardActor
    {
        private SummonData _data;
        
        public void SetSummonData(SummonData data, bool isRight)
        {
            _data = data;
            SetSprite(data.Sprite, isRight);
        }

        public override Vector3 GetMoveDirection()
        {
            return Vector3.right;
        }
    }
}
