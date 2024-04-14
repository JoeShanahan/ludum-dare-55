using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LudumDare55
{
    public class SummonAvatar : BoardActor
    {
        public SummonData SummonData { get; private set; }
        
        private int _queueIdx;
        private List<BoardAction> _actionQueue;    // TODO be able to overwrite if charmed, etc
        
        public void SetSummonData(SummonData data, bool isRight)
        {
            SummonData = data;
            _actionQueue = data.ActionQueue;
            SetSprite(data.Sprite, isRight);
        }

        public override void OnMoveComplete()
        {
            if (_board.IsSpaceValid(transform.localPosition) == false)
            {
                _board.ReturnToSender(this);
            }
        }

        public override void DoAction(float time)
        {
            base.DoAction(time);
            // Make sure it wraps back round to 0
    
            if (_actionQueue.Count > 0)
                _queueIdx = (_queueIdx + 1) % _actionQueue.Count;
        }

        public override void CommitToAction()
        {
            if (SummonData.ActionQueue.Count == 0)
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
