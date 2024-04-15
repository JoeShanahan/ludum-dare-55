using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare55
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private BookData _selectedBook;

        [SerializeField] private OpponentData _selectedOpponent;

        [SerializeField] private ActiveGameState _gameState;

        private void Start()
        {
            Application.targetFrameRate = 60;
            FindFirstObjectByType<MusicController>().SwapToMenuMusic();
        }
        
        public void BtnPressOpponent(OpponentData data)
        {
            _selectedOpponent = data;
        }

        public void BtnPressBook(BookData data)
        {
            _selectedBook = data;
        }

        public void BtnPressPlay()
        {
            _gameState.InitGame(_selectedBook, _selectedOpponent);
            FindFirstObjectByType<TransitionManager>().GoToGame();
            FindFirstObjectByType<MusicController>().SwapToGameMusic();
        }
    }
}
