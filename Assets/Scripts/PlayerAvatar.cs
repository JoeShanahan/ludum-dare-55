using UnityEngine;

namespace LudumDare55
{
    public class PlayerAvatar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        
        InputSystem_Actions _input;
        private bool _isPlayerControlled;

        private void Start()
        {
        }
        
        public void SetSprite(Sprite sprite, bool isRight)
        {
            _renderer.sprite = sprite;
            _renderer.flipX = isRight == false;
        }

        public void Init(bool isPlayerControlled)
        {
            
            _isPlayerControlled = isPlayerControlled;

            if (isPlayerControlled)
            {
                _input = new InputSystem_Actions();
                _input.Enable();
            }
        }
        
        void HandlePlayerInput()
        {
            Vector2 moveDir = _input.Player.Move.ReadValue<Vector2>();
            var playerInput = Vector2.ClampMagnitude(moveDir, 1);

            if (playerInput.magnitude < 0.3f)
                return;

            float horzMagnitude = Mathf.Abs(playerInput.x);
            float vertMagnitude = Mathf.Abs(playerInput.y);

            if (horzMagnitude > vertMagnitude)
            {
                DoMove(playerInput.x > 0 ? Vector3.right : Vector3.left);
            }
            else
            {
                DoMove(playerInput.y > 0 ? Vector3.up : Vector3.down);
            }
        }

        public void DoMove(Vector3 direction)
        {
            transform.localPosition += direction;
        }

        void Update()
        {
            if (_isPlayerControlled)
            {
                HandlePlayerInput();
            }
        }
    }
}
