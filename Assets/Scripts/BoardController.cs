using UnityEngine;

namespace LudumDare55
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject _squarePrefab;
        [SerializeField] private Color _colorA;
        [SerializeField] private Color _colorB;
        
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

            transform.position = new Vector3((1 - width) / 2f, (1 - height) / 2f, 0);
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            CreateBoard(9, 5);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
