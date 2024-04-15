using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LudumDare55
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform[] _buttonRects;
        [SerializeField] private Button _defaultButton;
        [SerializeField] private CanvasGroup _group;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        private void OnEnable()
        {
            _group.alpha = 0;
            _group.DOFade(1, 0.2f);
            
            
            EventSystem.current.SetSelectedGameObject(_defaultButton.gameObject);

            for (int i = 0; i < _buttonRects.Length; i++)
            {
                _buttonRects[i].localScale = Vector3.zero;
                _buttonRects[i].DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(0.1f + (0.04f * i));
            }
        }

        public void FadeAway()
        {
            _group.DOFade(0, 0.3f).SetDelay(0.2f).OnComplete(() => gameObject.SetActive(false));
            
            for (int i = 0; i < _buttonRects.Length; i++)
            {
                _buttonRects[i].DOScale(0, 0.3f).SetEase(Ease.InBack).SetDelay((0.03f * i));
            }
        }

        public void BtnPressRetry()
        {
            FindFirstObjectByType<TransitionManager>().GoToGame();
        }

        public void BtnPressMenu()
        {
            FindFirstObjectByType<TransitionManager>().GoToMenu();
        }
    }
}
