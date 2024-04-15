using UnityEngine;

namespace LudumDare55
{
    public class BattleMusicController : MonoBehaviour
    {
        [SerializeField] private ActiveGameState _gameState;
        [SerializeField] private AudioSource _audio;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // TODO audio setting
            _audio.clip = _gameState.Opponent.ThemeSong;
            
            if (_audio.clip != null)
                _audio.Play();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
