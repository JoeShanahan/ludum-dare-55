using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare55
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private BookData _selectedBook;

        [SerializeField] private OpponentData _selectedPlayer;
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

        public void BtnPressBookData(BookData data)
        {
            _selectedBook = data;
        }

        public void BtnPressBookPlayer(OpponentData player)
        {
            _selectedPlayer = player;
        }

        public void BtnPressPlay()
        {
            _gameState.InitGame(_selectedBook, _selectedPlayer, _selectedOpponent);
            FindFirstObjectByType<TransitionManager>().GoToGame();
            FindFirstObjectByType<MusicController>().SwapToGameMusic();
        }
    }
}
