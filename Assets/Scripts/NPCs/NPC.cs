using Assets.Scripts.BehaviourTree;
using Assets.Scripts.HitView;
using Assets.Scripts.Items;
using Assets.Scripts.Scenario;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPCs
{
    // Delegates for the events
    public delegate void OnNPCDeathEvent(NPC npc, HitMessage hitmessage);
    public delegate void OnNPCHitEvent(NPC npc, HitMessage hitmessage);

    /// <inheritdoc />
    /// <summary>
    /// Every NPC in the game should have this component.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPC : MonoBehaviour
    {
        public BodyType BodyType;
        public GameObject EquipHand;
        [SerializeField] private float _hp;

        private const float BASE_SPEED = 2.5f;
        private const float RUN_SPEED = 5.7f;
        private const float DEFAULT_STARTING_HP = 100.0f;
        private const float MIN_TIME_BEFORE_NERVOUS = 1.0f;
        private const float MAX_TIME_BEFORE_NERVOUS = 3.0f;

        public const string SURRENDER = "Surrender";

        /// <summary>
        /// List containing every NPC currently in the game.
        /// </summary>
        public static List<NPC> Npcs { get; private set; }

        /// <summary>
        /// List containing every hostile NPC currently in the game.
        /// </summary>
        public static List<NPC> HostileNpcs { get; private set; }

        /// <summary>
        /// list with all the waypoints selected in editor
        /// </summary>
        public List<Waypoint> Waypoints { get; set; }

        /// <summary>
        /// waypoint the NPC is heading towards
        /// </summary>
        public int CurrentWaypoint { get; set; }

        public Difficulty Difficulty { get; set; }

        public GameObject NervousSource { get; set; }

        /// <summary>
        /// Item the NPC is holding
        /// </summary>
        public Item Item { get; set; }

        public NPC TargetNpc { get; set; }

        public NavMeshAgent NavMeshAgent { get; private set; }
        public NavMeshObstacle NavMeshObstacle { get; private set; }
        public Animator Animator { get; private set; }
        public Ragdoll02 Ragdoll { get; private set; }

        public bool IsIdle { get; set; }

        public float lowerBodyLayer;

        private bool _oneHit;

        /// <summary>
        /// Amount of time that has passed at which this NPC has been held at gunpoint.
        /// Rate of decay equals the passage of time. If you want to increase this, 
        /// you would need to add at least more than deltaTime each tick.
        /// </summary>
        public float TimeHeldAtGunpoint { get; set; }

        public bool IsAlive { get; set; }
        public bool IsLyingDown { get; set; }
        public bool IsNervous { get; set; }
        [SerializeField]
        public bool IsPanicking { get; set; }
        public bool IsAttacking { get; set; }

        /// <summary>
        /// Returns true if running. When setting, also change the NPC max speed.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                NavMeshAgent.speed = _isRunning ? RUN_SPEED : BASE_SPEED;
            }
        }

        private bool _isRunning;

        /// <summary>
        /// Sets hostility of NPC. Also adds NPC to HostileNpcs or removes it. 
        /// </summary>
        public bool IsHostile
        {
            get { return _isHostile; }
            set
            {
                if (_isHostile == value) return;

                _isHostile = value;

                if (_isHostile)
                {
                    HostileNpcs.Add(this);
                }
                else
                {
                    HostileNpcs.Remove(this);
                }
            }
        }

        private bool _isHostile;

        /// <summary>
        /// The behaviour tree of this NPC. Should be updated every tick.
        /// </summary>
        private BT _bt;

        /// <summary>
        /// The amount of time that must pass before a NPC becomes nervous from being held at gunpoint.
        /// </summary>
        private float _timeBeforeNervous;

        // Events 
        public OnNPCDeathEvent OnNPCDeathEvent;
        public OnNPCHitEvent OnNPCHitEvent;

        static NPC()
        {
            // Initialize lists
            Npcs = new List<NPC>();
            HostileNpcs = new List<NPC>();
        }


        private void Awake()
        {
            Npcs.Add(this);
        }

        private void Start()
        {
            // Find and initialize different components
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshObstacle = GetComponent<NavMeshObstacle>();
            Animator = GetComponentInChildren<Animator>();
            Ragdoll = GetComponent<Ragdoll02>();
            IsAlive = true;

            OnNPCDeathEvent += OnDeathEvent;
            OnNPCHitEvent += OnHitEvent;

            _hp = DEFAULT_STARTING_HP;

            NavMeshAgent.enabled = true;

            _timeBeforeNervous = RNG.NextFloat(MIN_TIME_BEFORE_NERVOUS, MAX_TIME_BEFORE_NERVOUS);

            InitializeBehaviourTree();


        }
      

        private void Update()
        {
            if (!IsAlive) return;

            Animator.SetFloat("Speed", NavMeshAgent.velocity.magnitude);

            if (TimeHeldAtGunpoint > 0)
            {
                // Time held under gunshot has a standard rate of decay each tick.
                TimeHeldAtGunpoint = Mathf.Clamp(TimeHeldAtGunpoint - Time.deltaTime, 0, float.MaxValue);
            }

            if (TimeHeldAtGunpoint > _timeBeforeNervous)
            {
                IsNervous = true;
            }

            if (Animator.GetFloat("Speed") < Mathf.Abs(0.05f) && lowerBodyLayer != 1)
            {
                lowerBodyLayer = Mathf.Lerp(lowerBodyLayer, 0, 2 * Time.deltaTime);
                Animator.SetLayerWeight(1, lowerBodyLayer);
            }
            else
            {
                lowerBodyLayer = Mathf.Lerp(lowerBodyLayer, 1, Animator.GetFloat("Speed") * Time.deltaTime);
                Animator.SetLayerWeight(1, lowerBodyLayer);
            }

            if (_bt != null && Ragdoll.IsGettingUp == false)
            {
                _bt.Tick(); // Update AI Behavior Tree
            }
        }

        private void OnDestroy()
        {
            HostileNpcs.Remove(this);
            Npcs.Remove(this);
            if (Item != null)
            {
                Item.Dispose();
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// start the behaviourtree assigned to NPC
        /// </summary>
        private void InitializeBehaviourTree()
        {
            // Contains useful data for the behavior tree
            DataModel d = new DataModel(this);

            _bt = AIFactory.Instance.CreateBehaviourTree(Difficulty, d);
        }

        /// <summary>
        /// Handles damage from weapons. Is usually called by sending a message.
        /// </summary>
        /// <param name="hitMessage"></param>
        public void OnHit(HitMessage hitMessage)
        {
            if (!IsAlive) return;
            //only save hit if it comes from the player

            float damage = hitMessage.Damage;

            if (hitMessage.HitObject.name == "Head" || _oneHit)
            {
                damage *= 4;
            }

            _hp -= damage;


            if (_hp <= 0)
            {
                Die(hitMessage);
            }
            else if (Ragdoll._legsRag.Any(obj => obj.name == hitMessage.HitObject.name))
            {
                // NPC hit in leg, also not dead
                Ragdoll.ragFull = true;
                NavMeshAgent.enabled = false;
                Ragdoll.RagOn(hitMessage);
                Ragdoll.IsGettingUp = true;
                StartCoroutine(WaitGetUp());

                //wait until activating the oneHit kill boolean 
                StartCoroutine(WaitSetBool());
            }
            else
            {
                // NPC not hit in leg, also not dead
                if (!Ragdoll.IsGettingUp) //prevents unwanted blend Bug
                {
                    Ragdoll.ragFull = false;
                    Ragdoll.RagOn(hitMessage);
                }
            }

            OnNPCHitEvent(this, hitMessage);
        }

        /// <summary>
        /// Even that triggers when the npc gets hit
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="hitmessage"></param>
        private void OnHitEvent(NPC npc, HitMessage hitmessage)
        {
        }

        /// <summary>
        /// set player to instakill
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitSetBool()
        {
            yield return new WaitForSeconds(2);

            _oneHit = true;

            yield return new WaitForSeconds(6);

            _oneHit = false;
        }

        /// <summary>
        /// Wait a few seconds before getting up.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitGetUp()
        {
            yield return new WaitForSeconds(2);

            if (IsAlive)
            {
                StartCoroutine(Ragdoll.GetUp());
            }
            else
            {
                Ragdoll.stateType = StateType.One;
                Ragdoll.ragFull = true;
            }
        }

        /// <summary>
        /// Save the all the NPCs in the scene to a list
        /// </summary>
        /// <param name="hitmessage"></param>
        private void SaveHit(HitMessage hitmessage)
        {
            Debug.Log("Save hit");
            // Gameobject that got hit
            List<GameObject> tempList = new List<GameObject> {HitLocation.Save(this, hitmessage.PointOfImpact)};

            // Add rest of the npcs to the list
            foreach (NPC npc in Npcs)
            {
                // Return if current. to prevent the same NPC is added twice
                if (npc.gameObject.Equals(gameObject)) continue;
                tempList.Add(HitLocation.Save(npc));
            }

            ShowNpcHit.HitNPCs.Add(tempList);
        }

        /// <summary>
        /// Drop the weapon NPC is holding
        /// </summary>
        /// <param name="hitmessage"></param>
        /// <returns></returns>
        IEnumerator DropWeapon(HitMessage hitmessage)
        {
            yield return new WaitForSeconds(0.25f);

            Item.Instance.transform.parent = null;

            Rigidbody rbody = Item.Instance.GetComponent<Rigidbody>();

            rbody.isKinematic = false;
            Item.Instance.GetComponent<BoxCollider>().enabled = true;
            rbody.AddForceAtPosition(hitmessage.Source.TransformDirection(Vector3.up) * 300, hitmessage.PointOfImpact);
        }

        /// <summary>
        /// Make the NPC fall down and die
        /// </summary>
        /// <param name="hitMessage"></param>
        private void Die(HitMessage hitMessage)
        {
            IsAlive = false;

            //Activate LoseLimb Function of DetachableLimb.cs if Script is on hitObjects root. for Dummies
            if (hitMessage.HitObject.transform.root.GetComponent<DetachableLimbs>() != null)
            {
                hitMessage.HitObject.transform.root.SendMessage("LoseLimb", hitMessage,
                    SendMessageOptions.DontRequireReceiver);
            }
            //Ragdoll on Full
            Ragdoll.ragFull = true;
            Ragdoll.RagOn(hitMessage);
            Ragdoll.blend = false;

            //set item apart from owner
            if (Item != null && Item.Instance != null && Item.Instance.GetComponent<Rigidbody>() != null)
            {
                StartCoroutine(DropWeapon(hitMessage));
            }

            NavMeshAgent.enabled = false;
            NavMeshObstacle.enabled = true;

            if (IsHostile)
            {
                Statistics.DeadHostilesByPlayer++;
                HostileNpcs.Remove(this);
            }
            else
            {
                Statistics.DeadFriendliesByPlayer++;
            }


            OnNPCDeathEvent(this, hitMessage);
        }

        /// <summary>
        /// Even that is triggered when a NPC dies
        /// </summary>
        /// <param name="npc"></param>
        private void OnDeathEvent(NPC npc, HitMessage hitmessage)
        {
          
        }

        /// <summary>
        /// Clean up NPC and prepare for removal. Used indirectly by removing the GameObject of the NPC.
        /// </summary>
        public void Despawn()
        {
            Destroy(gameObject);
        }
    }
}