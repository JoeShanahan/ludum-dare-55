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
        [SerializeField] private RectTransform[] _heartInners;
        [SerializeField] private SummonAvatar _summon;

        public void Init(SummonAvatar avatar)
        {
            _summon = avatar;
            SetPosition(_summon.transform, Vector3.up * 2);
        }

        private void Update()
        {
            
        }
    }
}