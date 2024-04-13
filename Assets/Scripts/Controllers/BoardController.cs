using System;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare55
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _summonPrefab;
        [SerializeField] private GameObject _squarePrefab;
        [SerializeField] private Color _colorA;
        [SerializeField] private Color _colorB;

        [SerializeField] private Sprite _leftPlayerSprite;
        [SerializeField] private Sprite _rightPlayerSprite;
        
        public PlayerAvatar PlayerAvatar { get; private set; }
        public PlayerAvatar OpponentAvatar { get; private set; }

        private List<BoardActor> _allActors = new();
        private int _width;
        private int _height;
        
        public bool IsSpaceTaken(Vector3 pos)
        {
            Vector2Int intPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            
            foreach (BoardActor actor in _allActors)
            {
                if (actor.GridPosition == intPos)
                    return true;
            }
            
            return false;
        }

        public bool IsSpaceValid(Vector3 pos)
        {
            if (pos.x < 0 || pos.y < 0)
                return false;

            if (pos.x >= _width || pos.y >= _height)
                return false;
            
            return true;
        }
        
        public void MoveEverything(float moveTime)
        {
            foreach (BoardActor actor in _allActors)
            {
                actor.CommitToAction();
            }

            ResolveCollisions();
            
            foreach (BoardActor actor in _allActors)
            {
                // TODO just prepare to do the action, because it might have to change if there are collisions
                actor.DoAction(moveTime);
            }
        }
        
        public void CreateNewSummon(PlayerAvatar avatar, SummonData data)
        {
            GameObject newObj = Instantiate(_summonPrefab, transform);
            newObj.transform.localPosition = avatar.SummonPosition;
            
            SummonAvatar summon = newObj.GetComponent<SummonAvatar>();
            summon.SetSummonData(data, avatar.IsRight);
            _allActors.Add(summon);
        }

        public void InitBoard(int width, int height, BookData playerBook, BookData NpcBook)
        {
            CreateBoard(width, height);
            PlayerAvatar.SetBook(playerBook);
            OpponentAvatar.SetBook(NpcBook);
        }
        
        private void CreateBoard(int width, int height)
        {
            _allActors = new List<BoardActor>();
            _width = width;
            _height = height;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool isOdd = (x + y) % 2 == 0;

                    GameObject newObj = Instantiate(_squarePrefab, transform);
                    newObj.GetComponent<SpriteRenderer>().color = isOdd ? _colorA : _colorB;
                    newObj.transform.localPosition = new Vector3(x, y, 0);
                }
            }

            GameObject playerLeft = Instantiate(_playerPrefab, transform);
            GameObject playerRight = Instantiate(_playerPrefab, transform);
            
            playerLeft.transform.localPosition = new Vector3(0, (height - 1) / 2f, 0);
            playerRight.transform.localPosition = new Vector3(width - 1, (height - 1) / 2f, 0);

            PlayerAvatar = playerLeft.GetComponent<PlayerAvatar>();
            OpponentAvatar = playerRight.GetComponent<PlayerAvatar>();
            
            PlayerAvatar.SetSprite(_leftPlayerSprite, true);
            OpponentAvatar.SetSprite(_rightPlayerSprite, false);
            
            PlayerAvatar.Init(this);
            OpponentAvatar.Init(this);
            
            OpponentAvatar.EnableAI();
            
            _allActors.Add(PlayerAvatar);
            _allActors.Add(OpponentAvatar);
            
            transform.position = new Vector3((1 - width) / 2f, (1 - height) / 2f, 0);
        }

        public void OnMoveComplete()
        {
            OpponentAvatar.OnMoveComplete();
        }

        private void ResolveCollisions()
        {
            ResolveDirectBumps();
            ResolveSpaceDispute();
        }

        // For times when two actors are standing face to face and both trying to move forwards
        private void ResolveDirectBumps()
        {
            foreach (BoardActor actorA in _allActors)
            {
                foreach (BoardActor actorB in _allActors)
                {
                    if (actorA == actorB)
                        continue;

                    bool bothMoving = actorA.NextAction == SummonAction.Move && actorB.NextAction == SummonAction.Move;
                    bool AtoB = actorA.NextPosition == actorB.GridPosition;
                    bool BtoA = actorB.NextPosition == actorA.GridPosition;

                    if (bothMoving && AtoB && BtoA)
                    {
                        Debug.Log("Direct Bump Detected!");
                    }
                }
            }
        }
        
        // For times when two actors are standing one tile apart and both trying to go for the same tile
        private void ResolveSpaceDispute()
        {
            foreach (BoardActor actorA in _allActors)
            {
                foreach (BoardActor actorB in _allActors)
                {
                    if (actorA == actorB)
                        continue;

                    bool bothMoving = actorA.NextAction == SummonAction.Move && actorB.NextAction == SummonAction.Move;
                    bool bothSamePos = actorA.NextPosition == actorB.NextPosition;

                    if (bothMoving && bothSamePos)
                    {
                        Debug.Log("Space dispute Detected!");
                    }
                }
            }
        }
        
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
