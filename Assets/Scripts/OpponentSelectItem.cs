using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LudumDare55
{
    public class OpponentSelectItem : MonoBehaviour, ISelectHandler
    {
        public OpponentData OpponentData => _opponent;
        [SerializeField] private OpponentData _opponent;
        
        [SerializeField] private Image _personSprite;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _genreText;
        [SerializeField] private RectTransform _beatenRect;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetPersonData(_opponent);
        }
        
        //Do this when the selectable UI object is selected.
        public void OnSelect(BaseEventData eventData)
        {
            FindFirstObjectByType<NewMainMenuController>().OnOpponentSelected(this);
        }

        private void SetPersonData(OpponentData data)
        {
            _personSprite.sprite = data.Sprite;
            _nameText.text = data.PersonName;
            _genreText.text = data.GenreDescription;
            
            _beatenRect.gameObject.SetActive(HasBeaten);
        }

        private bool HasBeaten
        {
            get
            {
                string key = $"DebugHasBeaten{_opponent.PersonName}";
                return PlayerPrefs.GetInt(key, 0) == 1;
            }
            set
            {
                string key = $"DebugHasBeaten{_opponent.PersonName}";
                PlayerPrefs.SetInt(key, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        
        public void OnBtnPress()
        {
            FindFirstObjectByType<NewMainMenuController>().BtnPressOpponent(_opponent);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
