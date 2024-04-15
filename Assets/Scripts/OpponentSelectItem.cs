using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare55
{
    public class OpponentSelectItem : MonoBehaviour
    {
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

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
