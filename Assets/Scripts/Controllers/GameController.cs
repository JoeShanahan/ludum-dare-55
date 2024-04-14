using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare55
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BoardController _board;
        [SerializeField] private CardController _cards;
        [SerializeField] private float _moveTime = 0.5f;
        [SerializeField] private ActiveGameState _gameState;
        [SerializeField] private Transform _startBanner;
        
        private InputSystem_Actions _input;
        private bool _isMoving;

        public void GetNewCard()
        {
            if (_gameState.PlayerDeck.Count == 0)
                return;
            
            int randomIndex = Random.Range(0, _gameState.PlayerDeck.Count);
            SummonData newSummon = _gameState.PlayerDeck[randomIndex];
            _gameState.PlayerDeck.RemoveAt(randomIndex);
            _gameState.PlayerHand.Add(newSummon);
            
            _cards.Refresh();
        }

        public void ReturnToPlayerHand(SummonData s)
        {
            _gameState.PlayerHand.Add(s);
            _cards.Refresh();
        }

        public void RemoveCard(int pos)
        {
            _gameState.PlayerHand.RemoveAt(pos);
            _cards.Refresh();
        }
        
        private void Start()
        {
            _input = new InputSystem_Actions();
            _input.Enable();

            //_input.Player.Summon.performed += OnSummonPressed;
            _input.Player.SkipMove.performed += OnSkipPressed;

            _gameState.InitHands();
            _cards.Refresh();
            
            Application.targetFrameRate = 60;
            StartGame();
        }

        private void StartGame()
        {
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

        private void OnSkipPressed(InputAction.CallbackContext ctx)
        {
            if (_isMoving)
                return;
            
            TriggerBoardUpdate(Vector3.zero);
        }
        

        // Update is called once per frame
        void Update()
        {
            if (_isMoving)
                return;

            Vector3 moveDir = GetRequestedMoveDir();

            if (moveDir.magnitude == 0)
                return;

            bool valid = _board.IsSpaceValid(_board.PlayerAvatar.transform.localPosition + moveDir);

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
