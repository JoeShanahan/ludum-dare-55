using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LudumDare55
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _summonPrefab;
        [SerializeField] private GameObject _pagePrefab;
        [SerializeField] private GameObject _squarePrefab;
        [SerializeField] private ActiveGameState _gameState;
        
        [SerializeField] private Sprite _leftPlayerSprite;
        [SerializeField] private Sprite _rightPlayerSprite;
        
        public PlayerAvatar PlayerAvatar { get; private set; }
        public PlayerAvatar OpponentAvatar { get; private set; }
        public GameController GameController;

        private List<BoardActor> _allActors = new();
        public List<BoardActor> _pageActors { get; private set; } = new();
        private int _width;
        private int _height;
        private bool _areConflicts;

        private List<SummonAvatar> _summonsToReset = new();
        private List<SummonAvatar> _summonsToKill = new();
        private int _turnsUntilPage;
        
        public void ReturnToSender(SummonAvatar summon)
        {
            _summonsToReset.Add(summon);
        }
        
        public void OopsIDied(SummonAvatar summon)
        {
            _summonsToKill.Add(summon);
        }

        public BoardActor GetActorAt(Vector2Int pos)
        {
            foreach (BoardActor actor in _allActors)
            {
                if (actor.GridPosition == pos)
                    return actor;
            }

            return null;
        }
        
        public BoardActor GetActorGoingToBeAt(Vector2Int pos)
        {
            foreach (BoardActor actor in _allActors)
            {
                if (actor.NextPosition == pos)
                    return actor;
            }

            return null;
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
                actor.DoAction(moveTime);
            }

            StartCoroutine(CheckForAoeKills(moveTime));
        }

        private IEnumerator CheckForAoeKills(float moveTime)
        {
            yield return new WaitForSeconds(moveTime / 2);

            foreach (var hitInfo in _toHitWithAoe)
            {
                // TODO different thing for person and page
                if (hitInfo.Actor is SummonAvatar)
                {
                    hitInfo.Actor.Attack(hitInfo.Damage);
                }
            }
            
            _toHitWithAoe.Clear();
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
            _turnsUntilPage = 10;
        }
        
        private void CreateBoard(int width, int height)
        {
            _allActors = new List<BoardActor>();
            _width = width;
            _height = height;

            Vector3 midpoint = new Vector3((width-1) / 2f, (height-1) / 2f, 0);
            
            for (int x = 0; x < width; x++)
            {
                bool isEndTile = x == 0 || x == width - 1;
                
                for (int y = 0; y < height; y++)
                {
                    bool isOdd = (x + y) % 2 == 0;

                    GameObject newObj = Instantiate(_squarePrefab, transform);
                    OpponentData opp = _gameState.Opponent;
                    
                    Color col = isOdd ? opp._tileColA : opp._tileColB;

                    if (isEndTile)
                        col = Color.Lerp(col, Color.red, 0.15f);

                    newObj.GetComponent<SpriteRenderer>().color = col;
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
                    GameController.ReturnToPlayerHand(summon.SummonData);
                    Debug.Log("is returning");
                }
                else
                {
                    // TODO add to opponent hand
                }
            }

            foreach (SummonAvatar summon in _summonsToKill)
            {
                _allActors.Remove(summon);
            }
            
            _turnsUntilPage -= 1;
            if (_turnsUntilPage == 0) { SpawnPage(); }
        }

        private void ResolveCollisions()
        {
            // This is some hella inefficient code but who cares it's a gamejam
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
                        actorA.OverrideNextAction(BoardAction.X_HalfBounce, actorA.GridPosition);
                        actorB.OverrideNextAction(BoardAction.X_HalfBounce, actorB.GridPosition);
                        actorA.CollideWith(actorB);
                    }
                }
            }
        }

        private List<AoeTarget> _toHitWithAoe = new();

        private class AoeTarget
        {
            public BoardActor Actor;
            public int Damage;

            public AoeTarget(BoardActor actor, int damage)
            {
                Actor = actor;
                Damage = damage;
            }
        }

        public void DoAoeAttack(Vector2Int position, int damage, GameObject prefab)
        {
            Vector3 pos = new Vector3(position.x, position.y);
            
            if (IsSpaceValid(pos) == false)
                return;
            
            GameObject newObj = Instantiate(prefab, transform);
            newObj.transform.localPosition = pos;
            
            BoardActor onThisTile = GetActorGoingToBeAt(position);

            if (onThisTile != null)
            {
                Debug.LogWarning($"Gonna hit this guy with an AOE: {onThisTile.GridPosition}");
                _toHitWithAoe.Add(new AoeTarget(onThisTile, damage));
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
                    
                    bool isAWaiting = actorA.NextAction is BoardAction.Wait or BoardAction.AOEAttack;
                    bool isBWaiting = actorB.NextAction is BoardAction.Wait or BoardAction.AOEAttack;
                    
                    bool isAMoving = actorA.NextAction == BoardAction.Move;
                    bool isBMoving = actorB.NextAction == BoardAction.Move;

                    bool aAttackB = isAMoving && isBWaiting;
                    bool bAttackA = isBMoving && isAWaiting;

                    potential = aAttackB || bAttackA;

                    if (!potential)
                        continue;

                    bool areEnemies = actorA.IsRight != actorB.IsRight;
                    actorA.CollideWith(actorB);

                    if (areEnemies)
                    {
                        if (AtoB)
                        {
                            actorA.OverrideNextAction(BoardAction.X_Bounce, actorA.GridPosition);
                        }
                        if (BtoA)
                        {
                            actorB.OverrideNextAction(BoardAction.X_Bounce, actorB.GridPosition);
                        }
                    }
                    else
                    {
                        if (AtoB)
                        {
                            actorA.OverrideNextAction(BoardAction.Wait, actorA.GridPosition);
                        }
                        if (BtoA)
                        {
                            actorB.OverrideNextAction(BoardAction.Wait, actorA.GridPosition);
                        }
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

                    bool bothMoving = actorA.NextAction == BoardAction.Move && actorB.NextAction == BoardAction.Move;
                    bool bothSamePos = actorA.NextPosition == actorB.NextPosition;

                    if (bothMoving && bothSamePos)
                    {
                        // If can kill, do that and then move
                        // If being killed, move and die
                        // If not, bump
                        actorA.OverrideNextAction(BoardAction.X_Bounce, actorA.GridPosition);
                        actorB.OverrideNextAction(BoardAction.X_Bounce, actorB.GridPosition);
                        actorA.CollideWith(actorB);
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

        private void SpawnPage()
        {
            Vector2Int rPos = new Vector2Int();
            bool validPos = false;
            int i = 0;
            while (validPos == false)
            {
                i++;
                validPos = true;
                rPos = new Vector2Int(Random.Range(1, 8), Random.Range(0, 5));
                foreach (BoardActor a in _allActors)
                {
                    if (a.GridPosition == rPos) { validPos = false; }
                }
                if (i > 50)
                {
                    _turnsUntilPage = 5;
                    return;
                }
            }
            GameObject newObj = Instantiate(_pagePrefab, transform);
            newObj.transform.localPosition = new Vector3(rPos.x, rPos.y, 0f);
            PageAvatar page = newObj.GetComponent<PageAvatar>();
            page.IsPage = true;
            page.Init(this);
            _pageActors.Add(page);
            _turnsUntilPage = 10;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
