using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LudumDare55
{
    public class SummonCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _bodyText;
        [SerializeField] private Image _icon;

        private SummonData _data;
        private RectTransform _childRect;
        private CardStatsUI _statsUi;

        private void Start()
        {
            _childRect = transform.GetChild(0) as RectTransform;
            _childRect.anchoredPosition = new Vector2(0, -25);
            _statsUi = FindFirstObjectByType<CardStatsUI>();
        }
        
        public void SetSummonData(SummonData data)
        {
            _data = data;
            _titleText.text = data.Name;
            _bodyText.text = data.Description;
            _icon.sprite = data.Sprite;
        }

        public void OnCardPress()
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _childRect.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutExpo);
            _statsUi.SetSummon(_data);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _childRect.DOAnchorPosY(-25, 0.5f).SetEase(Ease.OutExpo);
            _statsUi.SetSummon(null);
        }
    }
}
