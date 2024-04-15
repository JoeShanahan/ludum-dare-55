using DG.Tweening;
using UnityEngine;

namespace LudumDare55
{
    [DefaultExecutionOrder(-5000)]
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private ActiveGameState _gameState;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip _menuMusic;

        [SerializeField] private float _maxVolume = 0.3f;
        [SerializeField] private float _fadeTime = 0.3f;
        private static MusicController _instance;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SwapToMenuMusic()
        {
            if (_audio.clip == _menuMusic)
                return;
            
            // TODO audio setting
            float volumeModifier = 1f;
            
            _audio.DOFade(0, _fadeTime).OnComplete(() =>
            {
                _audio.clip = _menuMusic;
                _audio.Play();
                _audio.DOFade(_maxVolume * volumeModifier, _fadeTime);
            });
        }
        
        public void SwapToGameMusic()
        {
            if (_audio.clip == _gameState.Opponent.ThemeSong)
                return;

            // TODO audio setting
            float volumeModifier = 1f;
            
            _audio.DOFade(0, _fadeTime).OnComplete(() =>
            {
                _audio.clip = _gameState.Opponent.ThemeSong;
                _audio.Play();
                _audio.DOFade(_maxVolume * volumeModifier, _fadeTime);
            });
        }
    }
}
