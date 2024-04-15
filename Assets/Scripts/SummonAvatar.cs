using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace LudumDare55
{
    public class SummonAvatar : BoardActor
    {
        public SummonData SummonData { get; private set; }

        [SerializeField] private GameObject _healthPrefab;

        private SummonHealthUI _spawnedUI;
        private int _queueIdx;
        private List<BoardAction> _actionQueue;    // TODO be able to overwrite if charmed, etc
        
        public void SetSummonData(SummonData data, bool isRight)
        {
            SummonData = data;
            _actionQueue = data.ActionQueue;
            SetSprite(data.Sprite, isRight);
            HealthPoints = data.HealthPoints;
            AttackDamage = data.Attack;
            ShieldPoints = data.Shield;
            
            _spawnedUI = W2C.InstantiateAs<SummonHealthUI>(_healthPrefab);
            _spawnedUI.Init(this); 
        }

        private void OnDestroy()
        {
            _spawnedUI.StopFollowing();
        }

        public override void Attack(int damage)
        {
            damage = Mathf.Max(damage - ShieldPoints, 1);
            
            HealthPoints -= damage;
            HealthPoints = Mathf.Max(HealthPoints, 0);

            if (HealthPoints == 0)
            {
                _board.OopsIDied(this);
                StartCoroutine(DeathRoutine());
            }
        }

        private IEnumerator DeathRoutine()
        {
            // Need to delay until end of frame before killing tween
            yield return null;
            
            Vector2Int deathDirection = -NextDirection;
            Vector3 deathPosition = transform.localPosition + new Vector3(deathDirection.x, deathDirection.y, 0);
                
            DOTween.Kill(transform);
            transform.DOLocalRotate(new Vector3(0, 0, 160), 0.3f).SetEase(Ease.Linear);
            transform.DOScale(0, 0.3f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
            transform.DOLocalMove(deathPosition, 0.3f).SetEase(Ease.OutSine);
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
        
        protected override void DoAoeAttack()
        {
            AttackArea aoeMode = SummonData.AttackArea;

            List<Vector2Int> positions = new();
            Vector2Int currentPos = GridPosition;

            int one = IsRight ? 1 : -1;

            // Around
            if (aoeMode is AttackArea.Sides or AttackArea.Circle or AttackArea.Cardinals)
            {
                positions.Add(currentPos + new Vector2Int(0, 1));
                positions.Add(currentPos + new Vector2Int(0, -1));
            }
            if (aoeMode is AttackArea.Circle or AttackArea.Cardinals)
            {
                positions.Add(currentPos + new Vector2Int(-one, 0));
                positions.Add(currentPos + new Vector2Int(one, 0));
            }
            if (aoeMode is AttackArea.Circle)
            {
                positions.Add(currentPos + new Vector2Int(-one, -1));
                positions.Add(currentPos + new Vector2Int(one, -1));
                positions.Add(currentPos + new Vector2Int(-one, 1));
                positions.Add(currentPos + new Vector2Int(one, 1));
            }
            
            // In front
            if (aoeMode is AttackArea.OneByOne or AttackArea.OneByTwo or AttackArea.OneByThree or AttackArea.ThreeByOne)
            {
                positions.Add(currentPos + new Vector2Int(one, 0));
            }
            if (aoeMode is AttackArea.OneByTwo)
            {
                positions.Add(currentPos + new Vector2Int(one + one, 0));
            }
            if (aoeMode is AttackArea.OneByThree)
            {
                positions.Add(currentPos + new Vector2Int(one + one, 0));
                positions.Add(currentPos + new Vector2Int(one + one + one, 0));
            }
            if (aoeMode is AttackArea.ThreeByOne)
            {
                positions.Add(currentPos + new Vector2Int(one, 1));
                positions.Add(currentPos + new Vector2Int(one, -1));
            }
            
            // full row/column
            if (aoeMode == AttackArea.Row)
            {
                for (int i = 1; i < 9; i++)
                {
                    positions.Add(new Vector2Int(currentPos.x + (one * i), currentPos.y));
                }
            }
            
            if (aoeMode == AttackArea.Column)
            {
                for (int i = 1; i < 5; i++)
                {
                    positions.Add(new Vector2Int(currentPos.x, currentPos.y - i));
                    positions.Add(new Vector2Int(currentPos.x, currentPos.y + i));
                }
            }


            foreach (Vector2Int pos in positions)
            {
                _board.DoAoeAttack(pos, AttackDamage, SummonData.AoePrefab);
            }
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
