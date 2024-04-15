using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare55
{
    public class CardStatsUI : MonoBehaviour
    {
        [SerializeField] private List<Image> _queueImages;
        [SerializeField] private List<Image> _heartImages;
        [SerializeField] private List<Image> _strengthImages;

        [Header("Icons")]
        [SerializeField] private Sprite _icoMove;
        [SerializeField] private Sprite _icoWait;
        [SerializeField] private Sprite _icoAttack;
        
        private GameObject _queueParent => _queueImages[0].transform.parent.gameObject;
        private GameObject _heartParent => _heartImages[0].transform.parent.gameObject;
        private GameObject _strengthParent => _strengthImages[0].transform.parent.gameObject;

        public void SetSummon(SummonData data)
        {
            if (data == null)
            {
                _queueParent.SetActive(false);
                _heartParent.SetActive(false);
                _strengthParent.SetActive(false);
                return;
            }
            
            _queueParent.SetActive(true);
            _heartParent.SetActive(true);
            _strengthParent.SetActive(data.Attack > 0);

            for (int i = 0; i < _queueImages.Count; i++)
            {
                _queueImages[i].gameObject.SetActive(data.ActionQueue.Count > i);
    
                if (_queueImages[i].gameObject.activeSelf)
                    _queueImages[i].sprite = GetQueueSprite(data.ActionQueue[i]);
            }
            
            for (int i = 0; i < _heartImages.Count; i++)
            {
                _heartImages[i].gameObject.SetActive(data.HealthPoints > i);
            }
            
            for (int i = 0; i < _strengthImages.Count; i++)
            {
                _strengthImages[i].gameObject.SetActive(data.Attack > i);
            }
        }

        private Sprite GetQueueSprite(BoardAction action)
        {
            return action switch
            {
                BoardAction.Move => _icoMove,
                BoardAction.Wait => _icoWait,
                BoardAction.AOEAttack => _icoAttack,
                _ => null
            };
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetSummon(null);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
