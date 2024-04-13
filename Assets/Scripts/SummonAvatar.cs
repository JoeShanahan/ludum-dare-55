using System.Collections.Generic;
using UnityEngine;

namespace LudumDare55
{
    public class SummonAvatar : BoardActor
    {
        private SummonData _data;
        private int _queueIdx;
        private List<BoardAction> _actionQueue;    // TODO be able to overwrite if charmed, etc
        
        public void SetSummonData(SummonData data, bool isRight)
        {
            _data = data;
            _actionQueue = data.ActionQueue;
            SetSprite(data.Sprite, isRight);
        }

 
        public override void DoAction(float time)
        {
            base.DoAction(time);
            // Make sure it wraps back round to 0
            _queueIdx = (_queueIdx + 1) % _actionQueue.Count;
        }

        public override void CommitToAction()
        {
            if (_data.ActionQueue.Count == 0)
                return;

            NextAction = _actionQueue[_queueIdx];

            Vector2Int forward = IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
            
            if (NextAction == BoardAction.Move)
            {
                NextPosition = GridPosition + forward;
                NextDirection = forward;
            }
            else if (NextAction == BoardAction.DoubleMove)
            {
                NextPosition = GridPosition + forward + forward;
                NextDirection = forward;
            }
        }
    }
}
