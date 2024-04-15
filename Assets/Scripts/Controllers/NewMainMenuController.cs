using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare55
{
    public class NewMainMenuController : MonoBehaviour
    {
        [SerializeField] private ActiveGameState _gameState;

        [SerializeField] private RectTransform _titleScreen;
        [SerializeField] private RectTransform _opponentScreen;
        [SerializeField] private RectTransform _bookScreen;
        
        private OpponentData _selectedOpponent;
        private BookData _selectedBook;

        public enum ScreenID { Title, PickOpponent, PickBook, Confirm, Loading }

        [SerializeField] private ScreenID _currentScreen;
        
        [SerializeField]
        private RectTransform _bgBlue;

        private InputSystem_Actions _input;
        
        private void OnEnable()
        {
            _input = new InputSystem_Actions();
            _input.Enable();
            _input.UI.Submit.performed += OnSubmitPressed;
            _input.UI.Cancel.performed += OnCancelPressed;
        }

        private void OnDisable()
        {
            _input.Disable();
            _input.UI.Submit.performed -= OnSubmitPressed;
            _input.UI.Cancel.performed -= OnCancelPressed;
        }

        private void OnSubmitPressed(InputAction.CallbackContext ctx)
        {
            if (_currentScreen == ScreenID.Title)
            {
                ShowOpponentSelect();
            }

            else if (_currentScreen == ScreenID.Confirm)
            {
                BtnPressPlay();
            }
        }
        
        private void OnCancelPressed(InputAction.CallbackContext ctx)
        {
            if (_currentScreen == ScreenID.PickOpponent)
            {
                ShowTitleScreen();
            }
            else if (_currentScreen == ScreenID.PickBook)
            {
                ShowOpponentSelect();
            }
            else if (_currentScreen == ScreenID.Confirm)
            {
                ShowBookSelect();
            }
        }


        private void Start()
        {
            BlueBgGoSmall(0);
            Application.targetFrameRate = 60;
            FindFirstObjectByType<MusicController>().SwapToMenuMusic();
        }

        private void ShowTitleScreen()
        {
            _currentScreen = ScreenID.Title;
            BlueBgGoSmall();
            _titleScreen.gameObject.SetActive(true);
            _opponentScreen.gameObject.SetActive(false);
        }

        private void ShowOpponentSelect()
        {
            _currentScreen = ScreenID.PickOpponent;
            BlueBgGoBig();
            _titleScreen.gameObject.SetActive(false);
            _opponentScreen.gameObject.SetActive(true);
        }

        private void ShowBookSelect()
        {
            _currentScreen = ScreenID.PickBook;
            _bookScreen.gameObject.SetActive(true);
            _opponentScreen.gameObject.SetActive(false);
        }

        private void ShowConfirm()
        {
            _currentScreen = ScreenID.Confirm;
        }

        private void BlueBgGoSmall(float time=0.5f)
        {
            DOTween.Kill(_bgBlue);
            _bgBlue.DOAnchorPosY(128, time).SetEase(Ease.OutExpo);
            Vector2 sd = _bgBlue.sizeDelta;
            sd.y = 180;
            _bgBlue.DOSizeDelta(sd, time).SetEase(Ease.OutExpo);
        }

        private void BlueBgGoBig(float time=0.5f)
        {
            DOTween.Kill(_bgBlue);
            _bgBlue.DOAnchorPosY(0, time).SetEase(Ease.OutExpo);

            Vector2 sd = _bgBlue.sizeDelta;
            sd.y = 575;
            _bgBlue.DOSizeDelta(sd, time).SetEase(Ease.OutExpo);
        }
        
        
        public void BtnPressOpponent(OpponentData data)
        {
            _selectedOpponent = data;
            ShowBookSelect();
        }

        public void BtnPressBookData(BookData data)
        {
            _selectedBook = data;
            ShowConfirm();
        }


        public void BtnPressPlay()
        {
            _gameState.InitGame(_selectedBook, _selectedOpponent, _selectedOpponent);
            FindFirstObjectByType<TransitionManager>().GoToGame();
            FindFirstObjectByType<MusicController>().SwapToGameMusic();
        }
    }
}
