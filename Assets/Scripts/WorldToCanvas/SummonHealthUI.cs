using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace LudumDare55
{
    public class SummonHealthUI : W2C
    {
        [SerializeField] private RectTransform[] _hearts;
        [SerializeField] private RectTransform[] _shields;
        private SummonAvatar _summon;
        private int _cachedHealth;
        private bool _isDestroying;
        
        public void Init(SummonAvatar avatar)
        {
            _summon = avatar;
            SetPosition(_summon.transform, Vector3.up * 0.4f);

            for (int i = 0; i < _shields.Length; i++)
            {
                _shields[i].gameObject.SetActive(avatar.ShieldPoints > i);
            }
            
            for (int i = 0; i < _hearts.Length; i++)
            {
                _hearts[i].gameObject.SetActive(avatar.HealthPoints > i);
            }

            _cachedHealth = avatar.HealthPoints;
        }

        private void OnEnable()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1, 0.4f).SetEase(Ease.OutExpo);
        }

        private void Update()
        {
            if (_isDestroying)
                return;
            
            if (_summon == null)
            {
                StopFollowing();
                transform.DOScale(0, 0.2f).SetEase(Ease.InSine).OnComplete(() => Destroy(gameObject));
                _isDestroying = true;
                return;
            }
            
            if (_summon.HealthPoints != _cachedHealth)
            {
                for (int i = 0; i < _hearts.Length; i++)
                {
                    if (_hearts[i].gameObject.activeSelf == false)
                        continue;

                    bool shouldBeVisible = _summon.HealthPoints > i;

                    Transform inner = _hearts[i].GetChild(0);

                    if (shouldBeVisible == false && inner.localScale.x > 0.98f)
                    {
                        inner.DOScale(0, 0.2f).SetEase(Ease.InSine);
                    }

                    if (shouldBeVisible && inner.localScale.x < 0.02f)
                    {
                        inner.DOScale(1, 0.2f).SetEase(Ease.OutExpo);
                    }
                }

                _cachedHealth = _summon.HealthPoints;
            }
        }
    }
}