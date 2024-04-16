using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LudumDare55
{
    public class ActiveGameState : ScriptableObject
    {
        public int StartHP;

        public BookData PlayerBook => _playerBook;
        public OpponentData Player => _player;
        public OpponentData Opponent => _opponent;

        public List<SummonData> PlayerHand { get; private set; } = new();
        public List<SummonData> OpponentHand { get; private set; } = new();
        
        public List<SummonData> PlayerDeck { get; private set; } = new();
        public List<SummonData> OpponentDeck { get; private set; } = new();

        public int PlayerSummonsInPlay;
        public int OpponentSummonsInPlay;

        public int PlayerHealth = new();
        public int OpponentHealth = new();

        private TextMeshProUGUI _playerHPUI;
        private TextMeshProUGUI _opponentHPUI;

        private StartFightBanner _winBan;
        private StartFightBanner _loseBan;
        
        [SerializeField] private BookData _playerBook;
        [SerializeField] private OpponentData _player;
        [SerializeField] private OpponentData _opponent;

        private bool _gameDone;
        
        public void InitGame(BookData playerBook, OpponentData player, OpponentData opponent)
        {
            _playerBook = playerBook;
            _player = player;
            _opponent = opponent;

            PlayerHealth = StartHP;
            OpponentHealth = StartHP;

            PlayerSummonsInPlay = 0;
            OpponentSummonsInPlay = 0;

            _gameDone = false;
        }

        public void InitUI(TextMeshProUGUI pHP, TextMeshProUGUI oHP, StartFightBanner wBan, StartFightBanner lBan)
        {
            _playerHPUI = pHP;
            _opponentHPUI = oHP;
            _winBan = wBan;
            _loseBan = lBan;

            RefreshUI();

        }

        public void RestartMatch()
        {
            InitGame(_playerBook, _player, _opponent);
        }

        public void InitHands()
        {
            Debug.Log($"Populating player with {_playerBook.name}");
            PopulateHandAndDeck(_playerBook, PlayerHand, PlayerDeck);
            PopulateHandAndDeck(_opponent.ChosenBook, OpponentHand, OpponentDeck);
            PlayerSummonsInPlay = PlayerHand.Count;
            OpponentSummonsInPlay = OpponentHand.Count;
        }

        public void DamagePlayer(bool isPlayer)
        {
            if (_gameDone) { return; }
            if (isPlayer)
            {
                PlayerHealth -= 1;
                RefreshUI();
                if (PlayerHealth == 0)
                {
                    _gameDone = true;
                    Debug.Log("Player lose!"); /*DO lose.*/
                    _loseBan.gameObject.SetActive(true);
                }
            }
            else
            {
                OpponentHealth -= 1;
                RefreshUI();
                if (OpponentHealth == 0)
                {
                    _gameDone = true;
                    Debug.Log("Player win!"); /*DO win.*/
                    _winBan.gameObject.SetActive(true);
                    
                    string key = $"DebugHasBeaten{_opponent.PersonName}";
                    PlayerPrefs.SetInt(key, 1);
                    PlayerPrefs.Save();
                }
            }

            Debug.LogWarning("DAMAGE DETECTED! pH:" + PlayerHealth + ", oH:" + OpponentHealth);
        }

        private void PopulateHandAndDeck(BookData book, List<SummonData> hand, List<SummonData> deck)
        {
            hand.Clear();
            deck.Clear();

            foreach (SummonData dat in book.Summons)
            {
                if (dat.DoesStartWith) { hand.Add(dat); }
                deck.Add(dat);
            }
        }

        private void RefreshUI()
        {
            _playerHPUI.text = PlayerHealth.ToString();
            _opponentHPUI.text = OpponentHealth.ToString();
        }
    }
}
