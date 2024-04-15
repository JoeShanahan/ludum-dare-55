using System.Collections.Generic;
using UnityEngine;

namespace LudumDare55
{
    public class CardController : MonoBehaviour
    {

        public PlayerAvatar Player;
        // TODO spawn the cards on match start according to the deck the player has
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private GameController _gameController;
        [SerializeField] private ActiveGameState _gameState;
        [SerializeField] private RectTransform _cardLayout;
        
        private List<SummonCard> _spawnedCards = new();
        
        public void Refresh()
        {
            // TODO the hand should be sorted in rarity order, or maybe numeric order
            while (_spawnedCards.Count < _gameState.PlayerHand.Count)
            {
                GameObject newObj = Instantiate(_cardPrefab, _cardLayout);
                var card = newObj.GetComponent<SummonCard>();
                card.CardController = this;
                _spawnedCards.Add(card);
            }

            for (int i = 0; i < _gameState.PlayerHand.Count; i++)
            {
                _spawnedCards[i].SetSummonData(_gameState.PlayerHand[i]);
                _spawnedCards[i].HandPos = i;
                _spawnedCards[i].CardController = this;
            }
        }

        public void RemoveCard(SummonCard card)
        {
            int x = card.HandPos;
            _spawnedCards.RemoveAt(x);
            _gameController.RemovePlayerCard(x);
            Destroy(card.gameObject);

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
