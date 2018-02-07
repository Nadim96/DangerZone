using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// Contains fields for describing a damage source.
    /// </summary>
    public struct HitMessage
    {
        /// <summary>
        /// Point of impact of damage source. For explosions this would be the origin of the explosion.
        /// </summary>
        public Vector3 PointOfImpact { get; set; }

        public Transform Source { get; set; }
        public GameObject HitObject { get; set; }
        public float Damage { get; set; }
        public float ImpactForce { get; set; }

        /// <summary>
        /// Does the damage originate from the player?
        /// </summary>
        public bool IsPlayer { get; set; }

        public HitMessage(Vector3 pointOfImpact, GameObject hitObject, Transform source, float damage,
            float impactForce, bool isPlayer = false) : this()
        {
            PointOfImpact = pointOfImpact;
            HitObject = hitObject;
            Source = source;
            Damage = damage;
            ImpactForce = impactForce;
            IsPlayer = isPlayer;
        }
    }
}