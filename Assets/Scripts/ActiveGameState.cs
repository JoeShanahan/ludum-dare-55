using System.Collections.Generic;
using UnityEngine;

namespace LudumDare55
{
    public class ActiveGameState : ScriptableObject
    {
        public BookData PlayerBook => _playerBook;
        public OpponentData Player => _player;
        public OpponentData Opponent => _opponent;

        public List<SummonData> PlayerHand { get; private set; } = new();
        public List<SummonData> OpponentHand { get; private set; } = new();
        
        public List<SummonData> PlayerDeck { get; private set; } = new();
        public List<SummonData> OpponentDeck { get; private set; } = new();
        
        [SerializeField] private BookData _playerBook;
        [SerializeField] private OpponentData _player;
        [SerializeField] private OpponentData _opponent;
        
        public void InitGame(BookData playerBook, OpponentData player, OpponentData opponent)
        {
            _playerBook = playerBook;
            _player = player;
            _opponent = opponent;
        }

        public void InitHands()
        {
            Debug.Log($"Populating player with {_playerBook.name}");
            PopulateHandAndDeck(_playerBook, PlayerHand, PlayerDeck);
            PopulateHandAndDeck(_opponent.ChosenBook, OpponentHand, OpponentDeck);
        }

        private void PopulateHandAndDeck(BookData book, List<SummonData> hand, List<SummonData> deck)
        {
            hand.Clear();
            deck.Clear();

            foreach (SummonData dat in book.Summons)
            {
                int count = dat.Count;

                if (dat.DoesStartWith)
                {
                    hand.Add(dat);
                    count--;
                }

                for (int i = 0; i < count; i++)
                {
                    deck.Add(dat);
                }
            }
        }
    }
}
