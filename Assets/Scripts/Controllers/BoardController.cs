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
        [SerializeField] private GameObject _basePrefab;
        [SerializeField] private ActiveGameState _gameState;
        
        [SerializeField] private Sprite _leftPlayerSprite;
        [SerializeField] private Sprite _rightPlayerSprite;
        
        public PlayerAvatar PlayerAvatar { get; private set; }
        public PlayerAvatar OpponentAvatar { get; private set; }
        public GameController GameController;

        public List<BoardActor> _allActors = new();
        public List<BoardActor> _summonActors { get; private set; } = new();
        public List<BoardActor> _pageActors { get; private set; } = new();
        private int _width;
        private int _height;
        private bool _areConflicts;

        private List<SummonAvatar> _summonsToReset = new();
        private List<SummonAvatar> _summonsToKill = new();

        private int _turnsUntilPlayerPage;
        private int _turnsUntilOpponentPage;
        
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
            _summonActors.Add(summon);
        }

        public void InitBoard(int width, int height, BookData playerBook, BookData NpcBook)
        {
            CreateBoard(width, height);
            PlayerAvatar.SetBook(playerBook);
            OpponentAvatar.SetBook(NpcBook);
            _turnsUntilPlayerPage = 10;
            _turnsUntilOpponentPage = 10;
        }
        
        private void CreateBoard(int width, int height)
        {
            _allActors = new List<BoardActor>();
            _width = width;
            _height = height;

            Vector3 midpoint = new Vector3((width-1) / 2f, (height-1) / 2f, 0);

            OpponentData ply = _gameState.Player;
            OpponentData opp = _gameState.Opponent;

            GameObject baseObj = Instantiate(_basePrefab, transform);
            /*baseObj.transform.eulerAngles = new Vector3(0, 0, 90);
            baseObj.transform.DOScale(1, 0.4f).SetEase(Ease.OutQuad).SetDelay(0f);
            baseObj.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutQuad).SetDelay(0f);*/

            for (int x = 0; x < width; x++)
            {
                //bool isEndTile = x == 0 || x == width - 1;
                
                for (int y = 0; y < height; y++)
                {
                    bool isOdd = (x + y) % 2 == 0;

                    GameObject newObj = Instantiate(_squarePrefab, transform);
                    if (x == 4) { newObj.GetComponent<SpriteRenderer>().color = isOdd ? opp._tileColC : opp._tileColD; }
                    else { newObj.GetComponent<SpriteRenderer>().color = isOdd ? opp._tileColA : opp._tileColB; }
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

            // _leftPlayerSprite = ply.sprite;
            _rightPlayerSprite = opp.sprite;
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
                _summonActors.Remove(summon);
                summon.transform.DOLocalRotate(new Vector3(0, 0, 160), 0.5f).SetEase(Ease.InSine);
                summon.transform.DOScale(0, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    var toRemove = summon.gameObject;
                    Destroy(toRemove);
                });

                _gameState.DamagePlayer(!summon.IsPlayer);
                if (summon.IsPlayer)
                {
                    GameController.ReturnToPlayerHand(summon.SummonData);
                    Debug.Log("is returning to ply");
                }
                else
                {
                    GameController.ReturnToOpponentHand(summon.SummonData);
                    Debug.Log("is returning to opp");
                }
            }

            foreach (SummonAvatar summon in _summonsToKill)
            {
                //Debug.Log("summontokill:" + summon.SummonData.Name + ",isplayer:" + summon.IsPlayer + ",summonkillssthisround:" + _summonsToKill.Count);
                if (summon.IsPlayer) { _gameState.PlayerSummonsInPlay -= 1; }
                else { _gameState.OpponentSummonsInPlay -= 1; }
                _allActors.Remove(summon);
                _summonActors.Remove(summon);
            }

            _summonsToKill.Clear();
            _turnsUntilPlayerPage -= 1;
            _turnsUntilOpponentPage -= 1;
            Debug.Log("pPageTurns:" + _turnsUntilPlayerPage);
            if (_turnsUntilPlayerPage <= 0) { SpawnPage(true); }
            if (_turnsUntilOpponentPage <= 0) { SpawnPage(false); }
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

                    bool AtoB = false;
                    bool BtoA = false;

                    bool bothMoving = (actorA.NextAction == BoardAction.Move || actorA.NextAction == BoardAction.DoubleMove)  && (actorB.NextAction == BoardAction.Move || actorB.NextAction == BoardAction.DoubleMove);
                    if (actorA.NextAction == BoardAction.Move && actorB.NextAction == BoardAction.Move)
                    {
                        AtoB = actorA.NextPosition == actorB.GridPosition;
                        BtoA = actorB.NextPosition == actorA.GridPosition;
                    }
                    else if (actorA.NextAction == BoardAction.DoubleMove && actorB.NextAction == BoardAction.DoubleMove)
                    {
                        Vector2Int forwardA = actorA.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        Vector2Int forwardB = actorB.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        AtoB = actorA.NextPosition - forwardA == actorB.GridPosition;
                        BtoA = actorB.NextPosition - forwardB == actorA.GridPosition;
                    }
                    else if (actorA.NextAction == BoardAction.DoubleMove && actorB.NextAction == BoardAction.Move)
                    {
                        Vector2Int forwardA = actorA.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        AtoB = actorA.NextPosition - forwardA == actorB.GridPosition;
                        BtoA = actorB.NextPosition == actorA.GridPosition;
                    }
                    else if (actorA.NextAction == BoardAction.Move && actorB.NextAction == BoardAction.DoubleMove)
                    {
                        Vector2Int forwardB = actorB.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        AtoB = actorA.NextPosition == actorB.GridPosition;
                        BtoA = actorB.NextPosition - forwardB == actorA.GridPosition;
                    }

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
                    
                    bool isAMoving = actorA.NextAction == BoardAction.Move || actorA.NextAction == BoardAction.DoubleMove;
                    bool isBMoving = actorB.NextAction == BoardAction.Move || actorB.NextAction == BoardAction.DoubleMove;

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
                            if (actorA.NextAction == BoardAction.DoubleMove)
                            {
                                Vector2Int forwardA = actorA.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                                actorA.OverrideNextAction(BoardAction.Move, actorA.NextPosition - forwardA);
                            } else { actorA.OverrideNextAction(BoardAction.X_Bounce, actorA.GridPosition); }
                        }
                        if (BtoA)
                        {
                            if (actorB.NextAction == BoardAction.DoubleMove)
                            {
                                Vector2Int forwardB = actorB.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                                actorB.OverrideNextAction(BoardAction.Move, actorB.NextPosition - forwardB);
                            } else { actorB.OverrideNextAction(BoardAction.X_Bounce, actorB.GridPosition); }
                        }
                    }
                    else
                    {
                        if (AtoB)
                        {                            
                            if (actorA.NextAction == BoardAction.DoubleMove)
                            {
                                Vector2Int forwardA = actorA.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                                actorA.OverrideNextAction(BoardAction.Move, actorA.NextPosition - forwardA);
                            } else { actorA.OverrideNextAction(BoardAction.Wait, actorA.GridPosition); }
                        }
                        if (BtoA)
                        {
                            if (actorB.NextAction == BoardAction.DoubleMove)
                            {
                                Vector2Int forwardB = actorB.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                                actorA.OverrideNextAction(BoardAction.Move, actorB.NextPosition - forwardB);
                            } else { actorB.OverrideNextAction(BoardAction.Wait, actorB.GridPosition); }
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

                    bool bothMoving = (actorA.NextAction == BoardAction.Move || actorA.NextAction == BoardAction.DoubleMove) && (actorB.NextAction == BoardAction.Move || actorB.NextAction == BoardAction.DoubleMove);
                    bool bothSamePos = actorA.NextPosition == actorB.NextPosition;

                    if (actorA.NextAction == BoardAction.Move && actorB.NextAction == BoardAction.Move)
                    {
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
                    else if (actorA.NextAction == BoardAction.DoubleMove && actorB.NextAction == BoardAction.DoubleMove)
                    {
                        Vector2Int forwardA = actorA.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        Vector2Int forwardB = actorB.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        if (bothMoving && bothSamePos)
                        {
                            actorA.OverrideNextAction(BoardAction.Move, actorA.NextPosition - forwardA);
                            actorB.OverrideNextAction(BoardAction.Move, actorB.NextPosition - forwardB);
                        }

                    }
                    else if (actorA.NextAction == BoardAction.DoubleMove && actorB.NextAction == BoardAction.Move)
                    {
                        Vector2Int forwardA = actorA.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        if (bothMoving && bothSamePos) { actorA.OverrideNextAction(BoardAction.Move, actorA.NextPosition - forwardA); }
                    }
                    else if (actorA.NextAction == BoardAction.Move && actorB.NextAction == BoardAction.DoubleMove)
                    {
                        Vector2Int forwardB = actorB.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                        if (bothMoving && bothSamePos) { actorB.OverrideNextAction(BoardAction.Move, actorB.NextPosition - forwardB); }
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

                    Vector2Int forwardA = actorA.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                    Vector2Int forwardB = actorB.IsRight ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);

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
                        if (actorA.NextAction == BoardAction.DoubleMove)
                        {
                            actorA.OverrideNextAction(BoardAction.Move, actorA.NextPosition - forwardA);
                        }
                        if (actorB.NextAction == BoardAction.DoubleMove)
                        {
                            actorB.OverrideNextAction(BoardAction.Move, actorB.NextPosition - forwardB);
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

        private void SpawnPage(bool isPlayer)
        {
            int plyPages = 0;
            int oppPages = 0;
            foreach (PageAvatar p in _pageActors) { if (p.IsPlayerPage) { plyPages += 1; } else { oppPages += 1; } }
            Debug.Log("plyPages:" + plyPages + ",oppPages:" + oppPages);
            if (plyPages < 2 && isPlayer == true)
            {
                Vector2Int rPos = new Vector2Int();
                bool validPos = false;
                int i = 0;
                while (validPos == false)
                {
                    i++;
                    validPos = true;
                    Random.InitState(System.DateTime.Now.Millisecond);
                    rPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 5));
                    foreach (BoardActor a in _allActors)
                    {
                        if (a.GridPosition == rPos) { validPos = false; }
                    }
                    if (i > 50)
                    {
                        _turnsUntilPlayerPage = 5;
                        return;
                    }
                }
                GameObject newObj = Instantiate(_pagePrefab, transform);
                newObj.transform.localPosition = new Vector3(rPos.x, rPos.y, 0f);
                PageAvatar page = newObj.GetComponent<PageAvatar>();
                page.IsPage = true;
                page.IsPlayerPage = true;
                page.Init(this);
                _pageActors.Add(page);
                _turnsUntilPlayerPage = 10;
            }

            if (oppPages < 2 && isPlayer == false)
            {
                Vector2Int rPos = new Vector2Int();
                bool validPos = false;
                int i = 0;
                while (validPos == false)
                {
                    i++;
                    validPos = true;
                    Random.InitState(System.DateTime.Now.Millisecond * 2);
                    rPos = new Vector2Int(Random.Range(5, 9), Random.Range(0, 5));
                    foreach (BoardActor a in _allActors)
                    {
                        if (a.GridPosition == rPos) { validPos = false; }
                    }
                    if (i > 50)
                    {
                        _turnsUntilOpponentPage = 5;
                        return;
                    }
                }
                GameObject newObj = Instantiate(_pagePrefab, transform);
                newObj.transform.localPosition = new Vector3(rPos.x, rPos.y, 0f);
                PageAvatar page = newObj.GetComponent<PageAvatar>();
                page.IsPage = true;
                page.IsPlayerPage = false;
                page.Init(this);
                _pageActors.Add(page);
                _turnsUntilOpponentPage = 10;
            }
        }

        public ActiveGameState GetState() { return _gameState; }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
