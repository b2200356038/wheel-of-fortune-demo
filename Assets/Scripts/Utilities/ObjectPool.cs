using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class ObjectPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform container;
        [SerializeField] private int initialSize = 10;
        [SerializeField] private int maxSize = 50;
        [SerializeField] private bool expandable = true;

        private Queue<GameObject> _availableObjects = new Queue<GameObject>();
        private List<GameObject> _allObjects = new List<GameObject>();

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (prefab == null)
            {
                Debug.LogError($"[ObjectPool] Prefab is not assigned on {gameObject.name}");
                return;
            }

            if (container == null)
                container = transform;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private GameObject CreateNewObject()
        {
            GameObject obj = Instantiate(prefab, container);
            obj.SetActive(false);
            _availableObjects.Enqueue(obj);
            _allObjects.Add(obj);
            return obj;
        }

        public GameObject Get()
        {
            if (_availableObjects.Count == 0)
            {
                if (expandable && _allObjects.Count < maxSize)
                {
                    return CreateNewObject();
                }
                
                Debug.LogWarning($"[ObjectPool] Pool is empty and cannot expand. Max size: {maxSize}");
                return null;
            }

            GameObject obj = _availableObjects.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null)
                return;

            if (!_allObjects.Contains(obj))
            {
                Debug.LogWarning($"[ObjectPool] Trying to return object that doesn't belong to this pool");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            
            if (!_availableObjects.Contains(obj))
            {
                _availableObjects.Enqueue(obj);
            }
        }

        public void ReturnAll()
        {
            foreach (var obj in _allObjects)
            {
                if (obj != null && obj.activeSelf)
                {
                    Return(obj);
                }
            }
        }

        public void Clear()
        {
            foreach (var obj in _allObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            
            _availableObjects.Clear();
            _allObjects.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }

        public int AvailableCount => _availableObjects.Count;
        public int TotalCount => _allObjects.Count;
        public int ActiveCount => _allObjects.Count - _availableObjects.Count;
    }
}