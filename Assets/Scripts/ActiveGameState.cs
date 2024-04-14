using UnityEngine;

namespace LudumDare55
{
    public class ActiveGameState : ScriptableObject
    {
        public BookData PlayerBook => _playerBook;
        public OpponentData Opponent => _opponent;

        [SerializeField] private BookData _playerBook;
        [SerializeField] private OpponentData _opponent;
        
        public void InitGame(BookData playerBook, OpponentData opponent)
        {
            _playerBook = playerBook;
            _opponent = opponent;
        }
    }
}
