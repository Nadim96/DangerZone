using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Audio;
using Assets.Scripts.BehaviourTree.Leaf.Conditions;
using Assets.Scripts.HitView;
using Assets.Scripts.NPCs;
using Assets.Scripts.Scenario;
using Assets.Scripts.UI;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <inheritdoc />
    /// <summary>
    /// Place this on a gun prefab for correct communication with a Weapon script.
    /// With this script the gun can fire.
    /// </summary>
    public class GunInterface : MonoBehaviour
    {
        [SerializeField] protected float _minDamage = DEFAULT_MIN_DAMAGE;
        [SerializeField] protected float _maxDamage = DEFAULT_MAX_DAMAGE;
        [SerializeField] protected float ImpactForce = DEFAULT_IMPACT_FORCE;
        [SerializeField] protected GameObject RaycastObject;
        [SerializeField] protected ParticleSystem MuzzleFlash;
        [SerializeField] protected Light MuzzleFlashLight;
        [SerializeField] protected GameObject BulletCasingSpawnLocation;
        [SerializeField] protected ParticleSystem SmokePuff;
        [SerializeField] protected ParticleSystem BloodOnHit;
        [SerializeField] protected ParticleSystem BulletHitStone;
        [SerializeField] protected ParticleSystem BulletHitMetal;
        [SerializeField] protected ParticleSystem BulletHitWood;

        public ObjectPool casingPool;

        protected const float DEFAULT_MIN_DAMAGE = 25;
        protected const float DEFAULT_MAX_DAMAGE = 25;
        protected const float DEFAULT_IMPACT_FORCE = 2000;

        protected Animator Animator;
        protected AudioSource AudioSource;

        protected virtual void Start()
        {
            Animator = GetComponent<Animator>();
            AudioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Specifically shoot a target. Gun orientation does not matter. Shot always hits the specified target.
        /// Sends HitMessage to target.
        /// </summary>
        /// <param name="target"></param>
        public void Shoot(GameObject target)
        {
            PlayGunEffects();
            if (target == null) return;
            Rigidbody rb = target.GetComponentsInChildren<Rigidbody>().ToList().First(x => x.name == "Spine1");
            float damage = RNG.NextFloat(_minDamage, _maxDamage);
            HitMessage message = new HitMessage(transform.position, rb.gameObject, RaycastObject.transform, damage,
                ImpactForce);
            SendHitMessage(target, message);
        }

        /// <summary>
        /// Shoot the gun. Uses the gun orientation and physics engine to determine what it hit. Sends HitMessage to target.
        /// </summary>
        public virtual void Shoot()
        {
            IsPanicking.playerShot = true;
            PlayGunEffects();

            Vector3 barrelDirection = RaycastObject.transform.TransformDirection(Vector3.up);

            RaycastHit hit;
            if (Physics.Raycast(RaycastObject.transform.position, barrelDirection, out hit))
            {
                // Adds shots to statistics
                ShowShots showShots = UnityEngine.Object.FindObjectOfType<ShowShots>();
                if (showShots != null && ScenarioBase.Instance.Started)
                {
                    showShots.Save(new Shot(hit.transform.gameObject, RaycastObject.transform.position, RaycastObject, hit.point), true);
                }

                HandleHit(hit);


            }

        }

        /// <summary>
        /// This method of shooting is a hybrid between the usage of the barrel orientation for hit detection,
        /// and the always-hit method. NPC's use this method to shoot at the player. Their bullets only hit
        /// if there is an unobstructed path between the gun of the NPC and the player's head.
        /// </summary>
        public void ShootAtPlayer()
        {
            PlayGunEffects();

            Transform playerEye = Player.Player.Instance.PlayerCameraEye;

            if (playerEye == null)
            {
                throw new NullReferenceException("Unable to locate player head, cannot shoot at player.");
            }

            RaycastHit hit;
            if (Physics.Linecast(RaycastObject.transform.position, playerEye.position, out hit))
            {
                if (hit.transform == playerEye)
                {
                    float damage = RNG.NextFloat(_minDamage, _maxDamage);
                    Player.Player.Instance.HandleHit(damage);
                }
                else
                {
                    HandleHit(hit);
                }

                // Adds shots to statistics
                ShowShots showShots = UnityEngine.Object.FindObjectOfType<ShowShots>();
                if (showShots != null)
                {
                    showShots.Save(new Shot(hit.transform.gameObject, RaycastObject.transform.position, this.gameObject.transform.GetComponentInParent<NPC>().gameObject, hit.point), false);
                }
            }
        }

        private void PlayGunEffects()
        {
            Animator.SetTrigger("Fire");
            AudioSource.volume = AudioController.GunVolume;
            AudioSource.Play();

            MuzzleFlash.Play();

            StartCoroutine(PlayMuzzleFlashLight(MuzzleFlash.main.duration));

            GameObject casingv2 = casingPool.GetObject();

            casingv2.transform.position = BulletCasingSpawnLocation.transform.position;
            casingv2.transform.rotation = BulletCasingSpawnLocation.transform.rotation;
            casingv2.SetActive(true);

            Rigidbody rb = casingv2.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(5000, 5000, 5000);

            float forward = RNG.NextFloat(0.4f, 1);
            float side = RNG.NextFloat(0.4f, 0.6f);

            rb.AddForce(transform.TransformDirection(new Vector3(-forward, 0, side)) * 1.5f);

            SmokePuff.Play(true);
        }

        private IEnumerator PlayMuzzleFlashLight(float duration)
        {
            MuzzleFlashLight.enabled = true;

            yield return new WaitForSeconds(duration);

            MuzzleFlashLight.enabled = false;
        }

        /// <summary>
        /// Checks tags of hit object to play environment effects or to send hit message to a game object.
        /// </summary>
        /// <param name="hit"></param>
        private void HandleHit(RaycastHit hit)
        {
            HitMessage message = new HitMessage(
                hit.point,
                hit.transform.gameObject,
                RaycastObject.transform,
                RNG.NextFloat(_minDamage, _maxDamage),
                ImpactForce,
                true
            );

            bool hitNPC = (hit.transform.gameObject.GetComponentInParent<NPC>() != null) ? true : false;
            if (hitNPC)
            {
                Statistics.ShotsHit++;
            }

            // Check tags to see if it hit the environment or something else
            switch (hit.transform.tag)
            {
                case "Stone":
                    Instantiate(BulletHitStone, hit.point, Quaternion.LookRotation(hit.normal));
                    AudioController.PlayAudioAtPoint(hit.point, AudioCategory.GunHitStone,
                        AudioController.GunVolume);
                    break;
                case "Metal":
                    Instantiate(BulletHitMetal, hit.point, Quaternion.Euler(hit.normal));
                    AudioController.PlayAudioAtPoint(hit.point, AudioCategory.GunHitMetal,
                        AudioController.GunVolume);
                    break;
                case "Wood":
                    Instantiate(BulletHitWood, hit.point, Quaternion.Euler(hit.normal));
                    AudioController.PlayAudioAtPoint(hit.point, AudioCategory.GunHitWood,
                        AudioController.GunVolume);
                    break;
                case "Civilian":
                    break;
                case "Bird":
                    hit.transform.SendMessage("OnHit", message, SendMessageOptions.DontRequireReceiver);
                    break;
            }

            // It is up to the receiver to implement a OnHit function.
            SendHitMessage(hit.transform.gameObject, message);
        }

        private void SendHitMessage(GameObject receiver, HitMessage message)
        {
            // Send this message to both root and self to ensure gameobject can receive message
            receiver.transform.root.SendMessage("OnHit", message, SendMessageOptions.DontRequireReceiver);
            receiver.transform.SendMessage("OnHit", message, SendMessageOptions.DontRequireReceiver);
        }
    }
}