using System.Collections.Generic;
using UnityEngine;

namespace LudumDare55
{
    public class SummonAvatar : BoardActor
    {
        private SummonData _data;
        private int _queueIdx;
        private List<SummonAction> _actionQueue;    // TODO be able to overwrite if charmed, etc
        
        public void SetSummonData(SummonData data, bool isRight)
        {
            _data = data;
            _actionQueue = data.ActionQueue;
            SetSprite(data.Sprite, isRight);
        }

 
        public override void DoAction(float time)
        {
            if (_data.ActionQueue.Count == 0)
                return;
            
            SummonAction currentAction = _data.ActionQueue[_queueIdx];

            if (currentAction == SummonAction.Move)
            {
                DoMove(Vector3.right, time);   
            }
            else if (currentAction == SummonAction.DoubleMove)
            {
                DoMove(Vector3.right * 2, time);
            }
            
            // Make sure it wraps back round to 0
            _queueIdx = (_queueIdx + 1) % _data.ActionQueue.Count;
        }
    }
}
