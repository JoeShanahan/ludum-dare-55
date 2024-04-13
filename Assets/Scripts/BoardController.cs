using UnityEngine;

namespace LudumDare55
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _squarePrefab;
        [SerializeField] private Color _colorA;
        [SerializeField] private Color _colorB;

        [SerializeField] private Sprite _leftPlayerSprite;
        [SerializeField] private Sprite _rightPlayerSprite;
        
        public void CreateBoard(int width, int height)
        {
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
            
            playerLeft.GetComponent<PlayerAvatar>().SetSprite(_leftPlayerSprite, true);
            playerRight.GetComponent<PlayerAvatar>().SetSprite(_rightPlayerSprite, false);
            
            playerLeft.GetComponent<PlayerAvatar>().Init(true);

            transform.position = new Vector3((1 - width) / 2f, (1 - height) / 2f, 0);
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Application.targetFrameRate = 60;
            CreateBoard(9, 5);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
