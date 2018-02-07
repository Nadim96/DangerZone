using System;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class EnableGameObjectOnStart : MonoBehaviour
    {
        [SerializeField] public GameObject GameObjectToEnable;

        private void Start()
        {
            if (GameObjectToEnable == null)
            {
                throw new NullReferenceException(
                    "Please ensure that EnableGameObjectOnStart has a reference to a GameObject.");
            }

            GameObjectToEnable.SetActive(true);
        }
    }
}