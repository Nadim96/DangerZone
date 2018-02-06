using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float Delay;

        private void Start()
        {
            StartCoroutine(Destroy(Delay));
        }

        void Update()
        {
            if (transform.position.y < -10)
                gameObject.SetActive(false);
        }

        private IEnumerator Destroy(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Deactivate to re add it to the objectpool
            gameObject.SetActive(false);
        }
    }
}