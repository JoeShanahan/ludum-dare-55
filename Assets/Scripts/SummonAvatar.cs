using System.Collections.Generic;
using DG.Tweening;
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
            HealthPoints = data.HealthPoints;
            AttackDamage = data.Attack;
        }

        public override void Attack(int damage)
        {
            HealthPoints -= damage;
            HealthPoints = Mathf.Max(HealthPoints, 0);

            if (HealthPoints == 0)
            {
                _board.OopsIDied(this);

                Vector2Int deathDirection = -NextDirection;
                Vector3 deathPosition = transform.localPosition + new Vector3(deathDirection.x, deathDirection.y, 0);
                
                DOTween.Kill(transform);
                transform.DOLocalRotate(new Vector3(0, 0, 160), 0.3f).SetEase(Ease.Linear);
                transform.DOScale(0, 0.3f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
                transform.DOLocalMove(deathPosition, 0.3f).SetEase(Ease.OutSine);
            }
        }

        private void OnEnable()
        {
            transform.localEulerAngles = new Vector3(0, 0, -160);
            transform.localScale = Vector3.zero;
            transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutExpo);
            transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
        }
        

        public override void OnMoveComplete()
        {
            if (_board.IsSpaceValid(transform.localPosition) == false)
            {
                _board.ReturnToSender(this);
            }

            CollidingActor = null;
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
