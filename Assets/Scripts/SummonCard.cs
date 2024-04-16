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
        [SerializeField] private bool _isInMenuScene;

        private RectTransform _childRect;
        private CardStatsUI _statsUi;

        public CardController CardController;
        public SummonData Data { get; private set; }
        public int HandPos;

        private void Start()
        {
            if (_isInMenuScene)
                return;
            
            _childRect = transform.GetChild(0) as RectTransform;
            _childRect.anchoredPosition = new Vector2(0, -25);
            _statsUi = FindFirstObjectByType<CardStatsUI>();
        }
        
        public void SetSummonData(SummonData data)
        {
            Data = data;
            _titleText.text = data.Name;
            _bodyText.text = data.Description;
            _icon.sprite = data.Sprite;
        }

        public void OnCardPress()
        {
            if (_isInMenuScene)
                return;

            if (CardController.Player.TrySummon(Data))
            {
                CardController.RemoveCard(this);
                _statsUi.SetSummon(null);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isInMenuScene)
                return;
            
            _childRect.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutExpo);
            _statsUi.SetSummon(Data);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isInMenuScene)
                return;
            
            _childRect.DOAnchorPosY(-25, 0.5f).SetEase(Ease.OutExpo);
            _statsUi.SetSummon(null);
        }
    }
}
