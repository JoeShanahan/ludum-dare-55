using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace LudumDare55
{
    public class AIController : MonoBehaviour
    {
        enum AIState
        {
            NORMAL,
            SEEKING,
            RETURNING
        }

        [SerializeField] private AIState _aiState;
        private ActiveGameState _gameState;
        private BoardController _board;
        private PlayerAvatar _playerAvatar;
        private bool _isMovingUp = true;
        private int[,] _grid;
        private List<Vector2Int> _pages;

        public void Init(PlayerAvatar p, BoardController b)
        {
            _playerAvatar = p;
            _board = b;
            _gameState = b.GetState();
            _aiState = AIState.NORMAL;
        }

        public SummonData GetHandCard()
        {
            List<SummonData> hand = _gameState.OpponentHand;
            Random.InitState(System.DateTime.Now.Millisecond);
            int i = Random.Range(0, hand.Count);
            return hand[i];
        }

        public void PagePickup() { }

        public Vector3 DetermineAIAction() //Returning a vector3 zero indicates a summoning
        {
            _grid = CheckGrid();
            _pages = CheckPages();

            if (_pages.Count > 0) { _aiState = AIState.SEEKING; } //AI is seeking the nearest page.
            else if (_playerAvatar.GridPosition.x != 8) { _aiState = AIState.RETURNING; } //AI is returning to it's normal x position.
            else { _aiState = AIState.NORMAL; } //AI is currently in it's natural x position and will try to move vertically and roll to summon (high % roll, just so they don't do the same thing every turn).

            Vector3 newPos = new Vector3();

            switch (_aiState)
            {
                case AIState.NORMAL:
                    newPos = PathfindNormal();
                    return newPos - transform.localPosition;
                case AIState.SEEKING:
                    newPos = PathfindSeeking();
                    return newPos - transform.localPosition;
                case AIState.RETURNING:
                    newPos = PathfindReturning();
                    return newPos - transform.localPosition;
            }
            return Vector3.zero;
        }

        public void AttemptSummon()
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            int i = Random.Range(0, 100);
            if (i > 40) { _playerAvatar.TrySummon(GetHandCard()); }
        }

        private int[,] CheckGrid()
        {
            int[,] gridArr = new int[9, 5];
            gridArr[0, 0] = 1000;
            gridArr[1, 0] = 1000;
            gridArr[2, 0] = 1000;
            gridArr[3, 0] = 1000;
            gridArr[4, 0] = 1000;
            foreach (BoardActor a in _board._summonActors)
            {
                Vector2Int aPos = a.GridPosition;
                //Debug.Log("aPos:"  + aPos.x + "," + aPos.y);
                gridArr[aPos.x, aPos.y] = 1000;
                if (a.IsRight && aPos.x != 8) { gridArr[aPos.x + 1, aPos.y] = 1000; continue; }
                if (!a.IsRight && aPos.x != 0) { gridArr[aPos.x - 1, aPos.y] = 1000; }
            }
            Vector2Int playerPos = _board.PlayerAvatar.GridPosition;
            //Debug.Log("pPos:" + playerPos);
            gridArr[playerPos.x, playerPos.y] = 1000;
            if (playerPos.x != 8) { gridArr[playerPos.x + 1, playerPos.y] = 1000; }
            return gridArr;
        }

        private List<Vector2Int> CheckPages()
        {
            List<Vector2Int> pList = new List<Vector2Int>();
            foreach (BoardActor p in _board._pageActors) { pList.Add(p.GridPosition); }
            return pList;
        }

        private Vector3 PathfindNormal()
        {
            Vector3 newPos = transform.localPosition;
            newPos += _isMovingUp ? Vector3.up : Vector3.down;
            if (_board.IsSpaceValid(newPos) == false)
            {
                _isMovingUp = !_isMovingUp;
                newPos = transform.localPosition;
                newPos += _isMovingUp ? Vector3.up : Vector3.down;
            }
            return newPos;
        }

        private Vector3 PathfindSeeking()
        {
            Vector2Int pos = _playerAvatar.GridPosition;
            Vector3 newPos = transform.localPosition;
            Vector2Int closestPage = new Vector2Int();
            int closestPageDist = 1000;
            foreach (Vector2Int page in _pages)
            {
                int dist = Math.Abs(pos.x - page.x) + Math.Abs(pos.y - page.y);
                if (dist < closestPageDist)
                {
                    closestPageDist = dist;
                    closestPage = page;
                }
            }

            if (closestPage.x > pos.x) //If the page is 'behind' the AI.
            {
                if (_grid[pos.x + 1, pos.y] != 1000 && _board.IsSpaceValid(newPos + Vector3.right)) { return newPos + Vector3.right; }
                if (closestPage.y > pos.y) //If the page is 'behind the AI' but they are blocked behind. Above
                {
                    if (_board.IsSpaceValid(newPos + Vector3.up) && _grid[pos.x, pos.y + 1] != 1000) { return newPos + Vector3.up; }
                    if (_board.IsSpaceValid(newPos + Vector3.right) && _grid[pos.x + 1, pos.y] != 1000) { return newPos + Vector3.right; }
                    if (_board.IsSpaceValid(newPos + Vector3.left) && _grid[pos.x - 1, pos.y] != 1000) { return newPos + Vector3.left; }
                    if (_board.IsSpaceValid(newPos + Vector3.down) && _grid[pos.x, pos.y - 1] != 1000) { return newPos + Vector3.down; }
                }
                else //Below.
                {
                    if (_board.IsSpaceValid(newPos + Vector3.down) && _grid[pos.x, pos.y - 1] != 1000) { return newPos + Vector3.down; }
                    if (_board.IsSpaceValid(newPos + Vector3.right) && _grid[pos.x + 1, pos.y] != 1000) { return newPos + Vector3.right; }
                    if (_board.IsSpaceValid(newPos + Vector3.left) && _grid[pos.x - 1, pos.y] != 1000) { return newPos + Vector3.left; }
                    if (_board.IsSpaceValid(newPos + Vector3.up) && _grid[pos.x, pos.y + 1] != 1000) { return newPos + Vector3.up; }
                }
            }
            else if (closestPage.x < pos.x) //Page must be in line or in front of the AI.
            {
                if (_grid[pos.x - 1, pos.y] != 1000 && _board.IsSpaceValid(newPos + Vector3.left)) { return newPos + Vector3.left; }
                if (closestPage.y > pos.y) //If the page is in front of the AI but they are blocked. Above.
                {
                    if (_board.IsSpaceValid(newPos + Vector3.up) && _grid[pos.x, pos.y + 1] != 1000) { return newPos + Vector3.up; }
                    if (_board.IsSpaceValid(newPos + Vector3.left) && _grid[pos.x - 1, pos.y] != 1000) { return newPos + Vector3.left; }
                    if (_board.IsSpaceValid(newPos + Vector3.right) && _grid[pos.x + 1, pos.y] != 1000) { return newPos + Vector3.right; }
                    if (_board.IsSpaceValid(newPos + Vector3.down) && _grid[pos.x, pos.y - 1] != 1000) { return newPos + Vector3.down; }
                }
                else //Below.
                {
                    if (_board.IsSpaceValid(newPos + Vector3.down) && _grid[pos.x, pos.y - 1] != 1000) { return newPos + Vector3.down; }
                    if (_board.IsSpaceValid(newPos + Vector3.left) && _grid[pos.x - 1, pos.y] != 1000) { return newPos + Vector3.left; }
                    if (_board.IsSpaceValid(newPos + Vector3.right) && _grid[pos.x + 1, pos.y] != 1000) { return newPos + Vector3.right; }
                    if (_board.IsSpaceValid(newPos + Vector3.up) && _grid[pos.x, pos.y + 1] != 1000) { return newPos + Vector3.up; }
                }
            }
            else //Page must be in line with AI.
            {
                if (closestPage.y > pos.y) //Above
                {
                    if (_board.IsSpaceValid(newPos + Vector3.up) && _grid[pos.x, pos.y + 1] != 1000) { return newPos + Vector3.up; }
                    if (_board.IsSpaceValid(newPos + Vector3.right) && _grid[pos.x + 1, pos.y] != 1000) { return newPos + Vector3.right; }
                    if (_board.IsSpaceValid(newPos + Vector3.left) && _grid[pos.x - 1, pos.y] != 1000) { return newPos + Vector3.left; }
                    if (_board.IsSpaceValid(newPos + Vector3.down) && _grid[pos.x, pos.y - 1] != 1000) { return newPos + Vector3.down; }
                }
                else //Below.
                {
                    if (_board.IsSpaceValid(newPos + Vector3.down) && _grid[pos.x, pos.y - 1] != 1000) { return newPos + Vector3.down; }
                    if (_board.IsSpaceValid(newPos + Vector3.right) && _grid[pos.x + 1, pos.y] != 1000) { return newPos + Vector3.right; }
                    if (_board.IsSpaceValid(newPos + Vector3.left) && _grid[pos.x - 1, pos.y] != 1000) { return newPos + Vector3.left; }
                    if (_board.IsSpaceValid(newPos + Vector3.up) && _grid[pos.x, pos.y + 1] != 1000) { return newPos + Vector3.up; }

                }
            }

            return newPos;
        }

        private Vector3 PathfindReturning()
        {
            Vector2Int pos = _playerAvatar.GridPosition;
            Vector3 newPos = transform.localPosition;

            if (_board.IsSpaceValid(newPos + Vector3.right) && _grid[pos.x + 1, pos.y] != 1000) { return newPos + Vector3.right; }
            if (_board.IsSpaceValid(newPos + Vector3.up) && _grid[pos.x, pos.y + 1] != 1000) { return newPos + Vector3.up; }
            if (_board.IsSpaceValid(newPos + Vector3.down) && _grid[pos.x, pos.y - 1] != 1000) { return newPos + Vector3.down; }
            if (_board.IsSpaceValid(newPos + Vector3.left) && _grid[pos.x - 1, pos.y] != 1000) { return newPos + Vector3.left; }

            return newPos;
        }
    }
}
