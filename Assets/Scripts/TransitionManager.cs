using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LudumDare55
{
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] private Image _coverImage;
        private int _sceneToLoad;

        private static bool _startCovered;

        private float _transitionTime = 0.3f;
        private float _transitionDelay = 0.02f;
        
        private const int LEFT = 0;
        private const int RIGHT = 1;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (_startCovered)
            {
                _coverImage.fillAmount = 1;
                _coverImage.fillOrigin = RIGHT;
                _coverImage.DOFillAmount(0, _transitionTime).SetEase(Ease.Linear).SetDelay(_transitionDelay);
            }
            else
            {
                _coverImage.fillAmount = 0;
            }
        }

        public void GoToMenu()
        {
            _coverImage.fillOrigin = LEFT;
            _coverImage.fillAmount = 0;
            _coverImage.DOFillAmount(1, _transitionTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                SceneManager.LoadScene(0);
            });
            _startCovered = true;
        }

        public void GoToGame()
        {
            _coverImage.fillOrigin = LEFT;
            _coverImage.fillAmount = 0;
            _coverImage.DOFillAmount(1, _transitionTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                SceneManager.LoadScene(1);
            });
            _startCovered = true;
        }
    }
}
