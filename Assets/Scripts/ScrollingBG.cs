using UnityEngine;
using UnityEngine.UI;

namespace LudumDare55
{
    public class ScrollingBG : MonoBehaviour
    {
        [SerializeField] private Vector2 _scrollSpeed;
        [SerializeField] private RawImage _image;
        [SerializeField] private Image _bgImage;
        [SerializeField] private ActiveGameState _gameState;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _image.texture = _gameState.Opponent.BackgroundTex;
            _bgImage.color = _gameState.Opponent._bgColA;
            _image.color = _gameState.Opponent._bgColB;
        }

        // Update is called once per frame
        void Update()
        {
            Rect uvRect = _image.uvRect;
            uvRect.x += Time.deltaTime * _scrollSpeed.x;
            uvRect.y += Time.deltaTime * _scrollSpeed.y;

            _image.uvRect = uvRect;
        }
    }
}
