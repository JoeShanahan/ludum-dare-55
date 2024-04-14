using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldToCanvas
{
    public class W2CManager : MonoBehaviour
    {
        static W2CManager _instance;
        [SerializeField] RectTransform _canvasRect;

        Canvas _canvas;     
        RectTransform _rect;
        Camera _camera;

        void Awake()
        {
            _instance = this;
            _rect = GetComponent<RectTransform>();
            _canvas = _canvasRect.GetComponent<Canvas>();
            _camera = Camera.main;
        }

        public static W2C Instantiate(GameObject prefab)
        {
            // Check we've set up the singleton first
            if (_instance == null)
            {
                Debug.LogError("W2CManager cannot instantiate because it doesn't yet exist in the scene!");
                return null;
            }

            // Instatiate the requested prefab, and get the W2C component
            GameObject spawnedObj = Instantiate(prefab, _instance._rect);
            W2C comp = spawnedObj.GetComponent<W2C>();

            // Immediately destroy the object if it's invalid
            if (comp == null)
            {
                Destroy(spawnedObj);
                Debug.LogError("W2CManager instantiated an object that doesnt have a W2C component on it!");
                return null;
            }

            // Assign the Canvas and Camera to it
            comp.Initialize(_instance._canvas, _instance._camera);

            return comp;
        }

        public static T InstantiateAs<T>(GameObject prefab) where T : W2C
        {
            return Instantiate(prefab) as T;
        }
    }
}
