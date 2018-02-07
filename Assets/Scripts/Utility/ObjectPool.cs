using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// this class is used to save objects in memory to prevent having to load them in all the time
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public GameObject Object;
        public bool CanGrow;
        public int PoolSize;

        private List<GameObject> pool;

        /// <summary>
        /// Count the number of gameobjects that are currently not in use
        /// </summary>
        public int ActiveSize
        {
            get
            {
                int size = 0;
                foreach (GameObject g in pool)
                {
                    if (g.activeInHierarchy)
                        size++;
                }
                return size;
            }
        }

        void Start () {
            pool = new List<GameObject>();

            for (int i = 0; i < PoolSize; i++)
            {
                GameObject tempObject = Instantiate(Object);
                tempObject.transform.parent = transform;
                tempObject.SetActive(false);
            }
        }

        /// <summary>
        /// get object from pool
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            foreach (GameObject o in pool)
            {   //first try to return first non-active object
                if (!o.activeInHierarchy)
                    return o;
            }

            if (CanGrow) 
            {   //create new object and add it to pool
                GameObject tempObject = Instantiate(Object);
                tempObject.transform.parent = transform;
                tempObject.SetActive(false);
                pool.Add(tempObject);
                return tempObject;
            }

            //if pool cant grow, just return null
            return null;
        }

        /// <summary>
        /// disable all objects in pool
        /// </summary>
        public void Reset()
        {
            foreach (GameObject o in pool)
                o.SetActive(false);
        }
    }
}