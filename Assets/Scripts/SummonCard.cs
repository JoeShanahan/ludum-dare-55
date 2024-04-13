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

        private SummonData _data;

        public void SetSummonData(SummonData data)
        {
            _data = data;
            _titleText.text = data.Name;
            _bodyText.text = data.Description;
            _icon.sprite = data.Sprite;
        }
    }
}
