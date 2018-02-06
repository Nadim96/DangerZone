using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// Causes the object this script is attached to to destroy itself after a number of seconds.
    /// </summary>
    public class ParticleDestroy : MonoBehaviour
    {
        /// <summary>
        /// The delay before the parent gameobject is destroyed
        /// </summary>
        [SerializeField] private float _delay = 5f;

        void Start()
        {
            Destroy(this.gameObject, _delay);
        }
    }
}