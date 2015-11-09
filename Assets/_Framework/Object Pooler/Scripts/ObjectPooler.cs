using UnityEngine;
using System.Collections.Generic;

namespace AndysUnityFramework
{
    public class ObjectPooler : MonoBehaviour
    {
        [Header("Prefabs")]
        [Tooltip("The prefab which will be pooled")]
        public GameObject _prefab;

        [Header("Configuration")]
        [Tooltip("Amount of objects to pool upon scene start")]
        [SerializeField]
        int _poolAmount = 0;

        [Tooltip("If checked, the object pooler will create new instances if no free objects exist")]
        [SerializeField]
        bool _expandable = false;

        // List of pooled objects
        List<GameObject> _pool;

        // Cached GameObject properties
        Transform _transform;

        public GameObject pooledObject
        {
            get
            {
                return _prefab;
            }
        }

        public int pooledAmount
        {
            get
            {
                return _poolAmount;
            }
        }

        public GameObject[] pooledObjects
        {
            get
            {
                return _pool.ToArray();
            }
        }

        public bool expandable
        {
            get
            {
                return _expandable;
            }
            set
            {
                _expandable = value;
            }
        }

        public void Awake()
        {
            // Cache used GameObject properties
            _transform = this.transform;

            // Ensure pool amount is resonable
            if (_poolAmount < 0)
            {
                _poolAmount = 0;
            }

            _pool = new List<GameObject>(_poolAmount);

            // Create objects and add them to the pool
            for (int i = 0; i < _pool.Capacity; ++i)
            {
                GameObject gameObject = InstantiateObject();

                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Creates an object within the pool, and 
        /// parents it to this object
        /// </summary>
        /// <returns>The pooled object</returns>
        GameObject InstantiateObject()
        {
            // Instanstiate object
            GameObject gameObject = Instantiate(_prefab) as GameObject;

            // Set parent as this object for organisation, and set name
            // so users within the editor know object is being pooled
            gameObject.transform.SetParent(_transform);
            gameObject.name = "(Pooled Object)" + _prefab.name;

            // Add object to pool list
            _pool.Add(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Returns object back to the pool, and sets it as inactive
        /// </summary>
        /// <param name="a_gameObject">Object to return back to the pool</param>
        public void FreeObject(GameObject a_gameObject)
        {
#if UNITY_EDITOR
            // Ensure this object is within the pool
            if (!_pool.Contains(a_gameObject))
            {
                Debug.LogWarning(
                    string.Format("({0}) Object '{1}' not found within pool, unable to free object back into pool", 
                    this.name, a_gameObject.name));

                return;
            }
#endif
            a_gameObject.SetActive(false);
            a_gameObject.transform.SetParent(this.transform);
        }

        /// <summary>
        /// Returns an object from within the pool which isn't in use.
        /// </summary>
        /// <returns>Object from within the pool</returns>
        public GameObject GetObject()
        {
            // Find object which isn't in use (not active)
            for (int i = 0; i < _pool.Count; ++i)
            {
                GameObject gameObject = _pool[i];

                if (!gameObject.activeInHierarchy &&
                    gameObject.transform.parent == _transform)
                {
                    // Activate object, and return it
                    gameObject.SetActive(true);
                    return gameObject;
                }
            }

            if (_expandable)
            {
                // Create new object and return it
                GameObject gameObject = InstantiateObject();
                return gameObject;
            }
            else
            {
                // Unable to find free object
                return null;
            }
        }
    }
}
