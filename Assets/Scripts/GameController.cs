using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare55
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BoardController _board;
        [SerializeField] private float _moveTime = 0.5f;
        [SerializeField] private BookData _playerChosenBook;
        
        private InputSystem_Actions _input;
        private bool _isMoving;

        private void Start()
        {
            _input = new InputSystem_Actions();
            _input.Enable();

            _input.Player.Summon.performed += OnSummonPressed;
        }

        private void OnSummonPressed(InputAction.CallbackContext ctx)
        {
            if (_isMoving)
                return;
            
            _board.CreateNewSummon(_board.PlayerAvatar, _playerChosenBook.Summons[0]);
        }
        

        // Update is called once per frame
        void Update()
        {
            if (_isMoving)
                return;

            Vector3 moveDir = GetRequestedMoveDir();

            if (moveDir.magnitude == 0)
                return;

            _board.PlayerAvatar.SetDesiredDirection(moveDir);
            _board.MoveEverything(_moveTime);
            _board.PlayerAvatar.ResetDesiredDirection();
            
            StartCoroutine(WaitForMoveFinish());
        }

        private IEnumerator WaitForMoveFinish()
        {
            _isMoving = true;
            yield return new WaitForSeconds(_moveTime);
            _isMoving = false;
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
