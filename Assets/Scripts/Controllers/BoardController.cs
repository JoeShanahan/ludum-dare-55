using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LudumDare55
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _summonPrefab;
        [SerializeField] private GameObject _squarePrefab;
        [SerializeField] private ActiveGameState _gameState;
        
        [SerializeField] private Sprite _leftPlayerSprite;
        [SerializeField] private Sprite _rightPlayerSprite;
        
        public PlayerAvatar PlayerAvatar { get; private set; }
        public PlayerAvatar OpponentAvatar { get; private set; }

        private List<BoardActor> _allActors = new();
        private int _width;
        private int _height;
        private bool _areConflicts;

        private List<SummonAvatar> _summonsToReset = new();
        
        public void ReturnToSender(SummonAvatar summon)
        {
            _summonsToReset.Add(summon);
        }
        
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
            summon.Init(this);
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

            Vector3 midpoint = new Vector3((width-1) / 2f, (height-1) / 2f, 0);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool isOdd = (x + y) % 2 == 0;

                    GameObject newObj = Instantiate(_squarePrefab, transform);
                    OpponentData opp = _gameState.Opponent;
                    newObj.GetComponent<SpriteRenderer>().color = isOdd ? opp._tileColA : opp._tileColB;
                    newObj.transform.localPosition = new Vector3(x, y, 0);

                    float distFromCenter = Vector3.Distance(midpoint, newObj.transform.localPosition);
                    newObj.transform.localScale = Vector3.zero;
                    newObj.transform.eulerAngles = new Vector3(0, 0, 90);

                    newObj.transform.DOScale(1, 0.4f).SetEase(Ease.OutQuad).SetDelay(distFromCenter * 0.1f);
                    newObj.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutQuad).SetDelay(distFromCenter * 0.1f);
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
            _summonsToReset.Clear();
            var actorCache = new List<BoardActor>(_allActors);
            
            foreach (BoardActor actor in actorCache)
            {
                actor.OnMoveComplete();
            }

            foreach (SummonAvatar summon in _summonsToReset)
            {
                _allActors.Remove(summon);
                summon.transform.DOLocalRotate(new Vector3(0, 0, 160), 0.5f).SetEase(Ease.InSine);
                summon.transform.DOScale(0, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    var toRemove = summon.gameObject;
                    Destroy(toRemove);
                });

                if (summon.IsPlayer)
                {
                    // TODO add to player hand
                }
                else
                {
                    // TODO add to opponent hand
                }
            }
        }

        private void ResolveCollisions()
        {
            for (int i = 0; i < 16; i++)
            {
                ResolveDirectBumps();
                ResolveSpaceDispute();
                CheckForAllyBlocking();
                ResolveWaiting();
                CheckForRemainingConflicts();

                if (_areConflicts == false)
                    break;
                
                if (i == 15)
                    Debug.LogWarning("Couldn't resolve all the conflicts!");
            }
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

                    bool bothMoving = actorA.NextAction == BoardAction.Move && actorB.NextAction == BoardAction.Move;
                    bool AtoB = actorA.NextPosition == actorB.GridPosition;
                    bool BtoA = actorB.NextPosition == actorA.GridPosition;

                    if (bothMoving && AtoB && BtoA)
                    {
                        // If can kill, do that and then move
                        // If not, half bump
                        actorA.OverrideNextAction(BoardAction.HalfBounce, actorA.GridPosition);
                        actorB.OverrideNextAction(BoardAction.HalfBounce, actorB.GridPosition);
                    }
                }
            }
        }
        
        private void ResolveWaiting()
        {
            foreach (BoardActor actorA in _allActors)
            {
                foreach (BoardActor actorB in _allActors)
                {
                    if (actorA == actorB)
                        continue;

                    bool AtoB = actorA.NextPosition == actorB.GridPosition;
                    bool BtoA = actorB.NextPosition == actorA.GridPosition;

                    bool potential = AtoB || BtoA;

                    if (!potential)
                        continue;
                    
                    bool isAWaiting = actorA.NextAction == BoardAction.Wait;
                    bool isBWaiting = actorB.NextAction == BoardAction.Wait;
                    
                    bool isAMoving = actorA.NextAction == BoardAction.Move;
                    bool isBMoving = actorB.NextAction == BoardAction.Move;

                    bool aAttackB = isAMoving && isBWaiting;
                    bool bAttackA = isBMoving && isAWaiting;

                    potential = aAttackB || bAttackA;

                    if (!potential)
                        continue;
                    
                    Debug.Log("Yeah this has pontench");
                    /*
                    if (bothMoving && AtoB && BtoA)
                    {
                        // If can kill, do that and then move
                        // If not, half bump
                        actorA.OverrideNextAction(BoardAction.HalfBounce, actorA.GridPosition);
                        actorB.OverrideNextAction(BoardAction.HalfBounce, actorB.GridPosition);
                    }
                    */
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

                    bool bothMoving = actorA.NextAction == BoardAction.Move && actorB.NextAction == BoardAction.Move;
                    bool bothSamePos = actorA.NextPosition == actorB.NextPosition;

                    if (bothMoving && bothSamePos)
                    {
                        // If can kill, do that and then move
                        // If being killed, move and die
                        // If not, bump
                        actorA.OverrideNextAction(BoardAction.Bounce, actorA.GridPosition);
                        actorB.OverrideNextAction(BoardAction.Bounce, actorB.GridPosition);
                    }
                }
            }
        }
        
        private void CheckForAllyBlocking()
        {
            foreach (BoardActor actorA in _allActors)
            {
                foreach (BoardActor actorB in _allActors)
                {
                    if (actorA == actorB)
                        continue;

                    bool bothSamePos = actorA.NextPosition == actorB.NextPosition;
                    bool bothSameSide = actorA.IsRight == actorB.IsRight;
                    
                    if (bothSamePos && bothSameSide)
                    {
                        if (actorA.NextAction == BoardAction.Move)
                        {
                            actorA.OverrideNextAction(BoardAction.Wait, actorA.GridPosition);
                        }
                        if (actorB.NextAction == BoardAction.Move)
                        {
                            actorB.OverrideNextAction(BoardAction.Wait, actorB.GridPosition);
                        }
                    }
                }
            }
        }

        private void CheckForRemainingConflicts()
        {
            _areConflicts = false;
            
            foreach (BoardActor actorA in _allActors)
            {
                foreach (BoardActor actorB in _allActors)
                {
                    if (actorA == actorB)
                        continue;

                    bool bothSamePos = actorA.NextPosition == actorB.NextPosition;

                    if (bothSamePos)
                    {
                        Debug.LogWarning($"{actorA.NextAction}, {actorB.NextAction}");
                        _areConflicts = true;
                        return;
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
