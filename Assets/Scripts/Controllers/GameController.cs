using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Random = UnityEngine.Random;

namespace LudumDare55
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BoardController _board;
        [SerializeField] private CardController _cards;
        [SerializeField] private float _moveTime = 0.5f;
        [SerializeField] private ActiveGameState _gameState;
        [SerializeField] private Transform _startBanner;
        
        [SerializeField] private BookCover _leftBookCover;
        [SerializeField] private BookCover _rightBookCover;
        [SerializeField] private Transform _pauseMenu;

        [SerializeField] private TextMeshProUGUI PlayerHP;
        [SerializeField] private TextMeshProUGUI OpponentHP;
        
        private InputSystem_Actions _input;
        private bool _isMoving;

        public void GetNewCard()
        {
            if (_gameState.PlayerSummonsInPlay >= 8) { return; }

            Random.InitState(System.DateTime.Now.Millisecond);
            List<SummonData> weightedList = new List<SummonData>();
            foreach (SummonData s in _gameState.PlayerDeck)
            {
                int count = s.Weight;
                while (count > 0)
                {
                    weightedList.Add(s);
                    count--;
                }
            }
            int randomIndex = Random.Range(0, weightedList.Count);
            Debug.Log("rdmInd: " + randomIndex);
            SummonData newSummon = _gameState.PlayerDeck[randomIndex];
            _gameState.PlayerHand.Add(newSummon);
            _gameState.PlayerSummonsInPlay += 1;
            _cards.Refresh();
        }

        public void GetNewOpponentCard()
        {
            if (_gameState.OpponentSummonsInPlay >= 8) { return; }

            Random.InitState(System.DateTime.Now.Millisecond);
            List<SummonData> weightedList = new List<SummonData>();
            foreach (SummonData s in _gameState.OpponentDeck)
            {
                int count = s.Weight;
                while (count > 0)
                {
                    weightedList.Add(s);
                    count--;
                }
            }
            int randomIndex = Random.Range(0, weightedList.Count);
            Debug.Log("rdmInd: " + randomIndex);
            SummonData newSummon = _gameState.PlayerDeck[randomIndex];
            _gameState.OpponentHand.Add(newSummon);
            _gameState.OpponentSummonsInPlay += 1;
        }

        public void ReturnToPlayerHand(SummonData s)
        {
            _gameState.PlayerHand.Add(s);
            _cards.Refresh();
        }

        public void RemovePlayerCard(int pos)
        {
            _gameState.PlayerHand.RemoveAt(pos);
            _cards.Refresh();
        }

        public void ReturnToOpponentHand(SummonData s)
        {
            _gameState.OpponentHand.Add(s);
        }

        public void RemoveOpponentCard(SummonData s)
        {
            _gameState.OpponentHand.Remove(s);
        }

        private void Start()
        {
            _input = new InputSystem_Actions();
            _input.Enable();

            //_input.Player.Summon.performed += OnSummonPressed;
            _input.Player.SkipMove.performed += OnSkipPressed;
            _input.Player.Menu.performed += OnMenuPressed;

            _gameState.InitUI(PlayerHP, OpponentHP);
            _gameState.InitHands();
            _cards.Refresh();
            
            Application.targetFrameRate = 60;
            StartGame();
        }

        private void OnDestroy()
        {
            _input.Disable();
            _input.Player.SkipMove.performed -= OnSkipPressed;
            _input.Player.Menu.performed -= OnMenuPressed;
        }

        private void StartGame()
        {
            _leftBookCover.SetBook(_gameState.PlayerBook);
            _rightBookCover.SetBook(_gameState.Opponent.ChosenBook);
            
            _board.InitBoard(9, 5, _gameState.PlayerBook, _gameState.Opponent.ChosenBook);
            _cards.Player = _board.PlayerAvatar;
            _startBanner.gameObject.SetActive(true);
        }

        /*private void OnSummonPressed(InputAction.CallbackContext ctx)
        {
            if (_isMoving)
                return;
            
            _board.PlayerAvatar.TrySummon();
        }*/
        
        
        private void OnMenuPressed(InputAction.CallbackContext ctx)
        {
            _pauseMenu.gameObject.SetActive(true);
        }


        private void OnSkipPressed(InputAction.CallbackContext ctx)
        {
            if (_pauseMenu.gameObject.activeSelf)
                return;

            if (_isMoving)
                return;
            
            TriggerBoardUpdate(Vector3.zero);
        }
        

        // Update is called once per frame
        void Update()
        {
            if (_pauseMenu.gameObject.activeSelf)
                return;
            
            if (_isMoving)
                return;

            Vector3 moveDir = GetRequestedMoveDir();

            if (moveDir.magnitude == 0)
                return;

            bool valid = _board.IsSpaceValid(_board.PlayerAvatar.transform.localPosition + moveDir);
            //Debug.Log(moveDir);
            if (_board.PlayerAvatar.transform.localPosition.x + moveDir.x >= 4) { valid = false; }

            if (valid)
                TriggerBoardUpdate(moveDir);
        }

        private void TriggerBoardUpdate(Vector3 moveDir)
        {
            
            _board.PlayerAvatar.SetDesiredDirection(moveDir);
            _board.MoveEverything(_moveTime);
            
            StartCoroutine(WaitForMoveFinish());
        }

        private IEnumerator WaitForMoveFinish()
        {
            _isMoving = true;
            yield return new WaitForSeconds(_moveTime);
            _isMoving = false;

            _board.OnMoveComplete();
        }
        
        private Vector3 GetRequestedMoveDir()
        {
            Vector2 moveDir = _input.Player.Move.ReadValue<Vector2>();
            var playerInput = Vector2.ClampMagnitude(moveDir, 1);

            if (playerInput.magnitude < 0.3f)
                return Vector3.zero;

            float horzMagnitude = Mathf.Abs(playerInput.x);
            float vertMagnitude = Mathf.Abs(playerInput.y);

            if (horzMagnitude > vertMagnitude)
            {
                return playerInput.x > 0 ? Vector3.right : Vector3.left;
            }
            
            return playerInput.y > 0 ? Vector3.up : Vector3.down;
        }
    }
}
