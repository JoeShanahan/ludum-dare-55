using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare55
{
    public class SummonCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _bodyText;
        [SerializeField] private Image _icon;

        public CardController CardController;
        public SummonData Data { get; private set; }
        public int HandPos;

        public void SetSummonData(SummonData data)
        {
            Data = data;
            _titleText.text = data.Name;
            _bodyText.text = data.Description;
            _icon.sprite = data.Sprite;
        }

        public void OnCardPress()
        {
            if (CardController.Player.TrySummon(Data)) { CardController.RemoveCard(this); }
        }
    }
}
