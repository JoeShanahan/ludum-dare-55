using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LudumDare55
{
    public class NewMainMenuController : MonoBehaviour
    {
        [SerializeField] private ActiveGameState _gameState;

        [SerializeField] private RectTransform _titleScreen;
        [SerializeField] private RectTransform _opponentScreen;
        [SerializeField] private RectTransform _bookScreen;
        [SerializeField] private RectTransform _readyScreen;

        [SerializeField] private List<OpponentSelectItem> _opps;
        [SerializeField] private List<BookSelectItem> _boops;

        [SerializeField] private TMP_Text _bookBlurb;
        [SerializeField] private List<SummonCard> _cards;
        [SerializeField] private Image _oppImage;
        [SerializeField] private TMP_Text _battleText;
        
        private int _opIdx;
        private int _bookIdx;
        
        private OpponentData _selectedOpponent;
        private BookData _selectedBook;

        public enum ScreenID { Title, PickOpponent, PickBook, Confirm, Loading }

        [SerializeField] private ScreenID _currentScreen;
        
        [SerializeField]
        private RectTransform _bgBlue;

        private InputSystem_Actions _input;

        public void OnBookSelected(BookSelectItem itm)
        {
            _bookIdx = _boops.IndexOf(itm);
            _bookBlurb.text = itm.BookData.LongDescription;
            
            for (int i = 0; i < 5; i++)
            {
                _cards[i].SetSummonData(itm.BookData.Summons[i]);
            }
        }
        
        public void OnOpponentSelected(OpponentSelectItem itm)
        {
            _opIdx = _opps.IndexOf(itm);
        }
        
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
            if (_currentScreen == ScreenID.Confirm)
            {
                BtnPressPlay();
            }
            StartCoroutine(DelayedSubmit());
        }

        private IEnumerator DelayedSubmit()
        {
            yield return null;
            if (_currentScreen == ScreenID.Title)
            {
                ShowOpponentSelect();
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
            _bookScreen.gameObject.SetActive(false);
            
            EventSystem.current.SetSelectedGameObject(_opps[_opIdx].gameObject);
        }

        private void ShowBookSelect()
        {
            foreach (BookSelectItem itm in _boops)
            {
                itm.gameObject.SetActive(_selectedOpponent.ChosenBook != itm.BookData);
            }
            _currentScreen = ScreenID.PickBook;
            _bookScreen.gameObject.SetActive(true);
            _opponentScreen.gameObject.SetActive(false);
            _readyScreen.gameObject.SetActive(false);
            BlueBgGoBig();

            if (_boops[_bookIdx].gameObject.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(_boops[_bookIdx].gameObject);
            }
            else
            {
                foreach (var itm in _boops)
                {
                    if (itm.gameObject.activeSelf)
                    {
                        EventSystem.current.SetSelectedGameObject(itm.gameObject);
                        break;
                    }
                }
            }
        }

        private void ShowConfirm()
        {
            _oppImage.sprite = _selectedOpponent.sprite;
            _battleText.text = $"{_selectedBook.Genre} <size=20>vs</size> {_selectedOpponent.ChosenBook.Genre}";
            _currentScreen = ScreenID.Confirm;
            _bookScreen.gameObject.SetActive(false);
            _readyScreen.gameObject.SetActive(true);
            BlueBgGoSmall2();

        }

        private void BlueBgGoSmall(float time=0.5f)
        {
            DOTween.Kill(_bgBlue);
            _bgBlue.DOAnchorPosY(128, time).SetEase(Ease.OutExpo);
            Vector2 sd = _bgBlue.sizeDelta;
            sd.y = 180;
            _bgBlue.DOSizeDelta(sd, time).SetEase(Ease.OutExpo);
        }
        
        private void BlueBgGoSmall2(float time=0.5f)
        {
            DOTween.Kill(_bgBlue);
            _bgBlue.DOAnchorPosY(0, time).SetEase(Ease.OutExpo);
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
