using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BehaviourTree.Leaf.Conditions;
using Assets.Scripts.Items;
using Assets.Scripts.NPCs;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Scenario
{
    public class ScenarioBase : MonoBehaviour
    {
        protected ILoad LoadStyle;

        /// <summary>
        /// List of prefabs of NPC's that can be spawned.
        /// </summary>
        [SerializeField] protected List<Transform> PersonTargetPrefabs;

        /// <summary>
        /// The prefab of the paper target. 
        /// </summary>
        [SerializeField] private Transform PaperTargetPrefab;

        /// <summary>
        /// The prefab of the dummy target. 
        /// </summary>
        [SerializeField] protected Transform DummyTargetPrefab;

        /// <summary>
        /// The prefab of a waypoint. Load script uses this prefab 
        /// to instantiate waypoints beforehand.
        /// </summary>
        [SerializeField] private Transform WaypointPrefab;

        /// <summary>
        /// Gameobject all waypoints will be a child to
        /// </summary>
        [SerializeField] private Transform WaypointsObject;

        ///<summary>
        /// automaticly start scene
        /// </summary>
        [SerializeField] private bool StartOnLoad;

        public GameObject PlayerCameraEye;
        public Camera PlayerCameraEyeCamera;

        public float minTimeElapsedBeforeAttack = 5f;
        public float maxTimeElapsedBeforeAttack = 15f;

        /// <summary>
        /// Name of scene
        /// </summary>
        public string Name { get; set; }

        public TargetType TargetType { get; set; }
        public GoalType GoalType { get; set; }
        public List<Target> Targets { get; private set; }


        /// <summary>
        /// Reasons why a stage has ended
        /// </summary>
        public enum StageEndReason
        {
            AgentDied,
            CivilianDied,
            Succes
        }
        /// <summary>
        /// amount of target still alive in scene
        /// </summary>
        public int AliveTargets
        {
            get { return Targets.Count(x => x.IsAlive); }
        }

        public GameObject ingameUITrigger;
        public GameObject IngameUI;
        public GameObject GameOverScreen;
        public Text GameOverScreenText;

        /// <summary>
        /// Timestamp of when Scenario is started
        /// </summary>
        public float ScenarioStartedTime { get; protected set; }

        /// <summary>
        /// seconds to wait before Attack can start
        /// </summary>
        public float timeBeforeAttack;

        public bool Started;
        public bool AttackTriggered;
        public bool EnableIngameMenu;

        public static ScenarioBase Instance { get; private set; }

        public ScenarioBase()
        {
            Targets = new List<Target>();
            Instance = this;
        }

        /// <summary>
        /// set if the scenario is random or from xml
        /// </summary>
        public void SetLoadType()
        {
            LoadStyle = new LoadRandom();
        }

        /// <summary>
        /// Loads selected scenario 
        /// </summary>
        protected virtual void Load()
        {
            LoadStyle.Load();
        }

        protected virtual void Create()
        {
            LoadStyle.Create();
        }

        protected virtual void Start()
        {
#if UNITY_EDITOR
            Audio.AudioController.LoadAudio();
#endif

            if (PersonTargetPrefabs == null || PersonTargetPrefabs.Count == 0)
                throw new Exception("Scenario will not be able to spawn NPC's. " +
                                    "Please ensure the Spawnable NPC list is filled.");
            SetLoadType();

            if (StartOnLoad)
                Play();
        }

        public virtual void SetIngameUIVisible()
        {
            EnableIngameMenu = true;
        }

        protected virtual void Update()
        {
            MeshRenderer meshRenderer = ingameUITrigger.GetComponent<MeshRenderer>();

            //check endgame 
            if (NPC.HostileNpcs.Count == 0 && Started)
            {
                GameOver();
            }

            //enable menu when not looking
            if (EnableIngameMenu && !meshRenderer.isVisible)
            {
                IngameUI.SetActive(true);
                EnableIngameMenu = false;
            }
            else if (EnableIngameMenu && Time.time > ScenarioStartedTime + 2) //always enable after 2 second
            {
                IngameUI.SetActive(true);
                EnableIngameMenu = false;
            }

            //trigger attack
            if (Started && !AttackTriggered)
            {
                if (Time.time > ScenarioStartedTime + timeBeforeAttack)
                {
                    AttackTriggered = true;
                }
            }
        }

        /// <summary>
        /// load objects and start the scene
        /// </summary>
        public virtual void Play()
        {
            HideGameOverReason();
            IsPanicking.playerShot = false;
            //stop old scenario if it isnt stopped yet
            if (ScenarioStartedTime != 0)
            {
                Stop();
            }
            Load();
            Create();
            Spawn();
            ScenarioStartedTime = Time.time;
            Started = true;
            PlayerCameraEye.GetComponent<Player.Player>().Health = 100;
            timeBeforeAttack = RNG.NextFloat(minTimeElapsedBeforeAttack, maxTimeElapsedBeforeAttack);
            Time.timeScale = 1f;

            Statistics.Reset();
            Statistics.Show(false);
        }

        /// <summary>
        /// trigger gameover settings
        /// </summary>
        public virtual void GameOver()
        {
            Started = false;

            //StartCoroutine("gameoverWait", false);
            bool dead = NPC.HostileNpcs.All(hostileNpc => !hostileNpc.IsAlive);

            StartCoroutine("gameoverWait", dead);
        }

        /// <summary>
        /// activate gameover settings
        /// </summary>
        /// <param name="dead"></param>
        /// <returns></returns>
        private IEnumerator gameoverWait(bool dead)
        {
            if (dead)
                yield return new WaitForSeconds(2);
            if (Started) yield break;
            Scenario.GameOver.instance.SetEndscreen(dead);
            Time.timeScale = 0.0f; // Set time still

            Statistics.Show(true);
        }

        /// <summary>
        /// stop a game in progress
        /// </summary>
        public virtual void Stop()
        {
            Started = false;
            AttackTriggered = false;
            ScenarioStartedTime = 0;
            Time.timeScale = 1;
            Scenario.GameOver.instance.HideEndScreen();

            foreach (Target t in Targets)
                t.Destroy();
            Targets = new List<Target>();
        }

        public void BackToMainMenu(float delay)
        {
            StartCoroutine(BackToMainMenuCoroutine(delay));
        }

        private IEnumerator BackToMainMenuCoroutine(float delay)
        {
            Stop();

            yield return new WaitForSecondsRealtime(delay); //realtime so it isnt affected by timescale 0

            var task = SceneManager.LoadSceneAsync("MainMenu");

            while (!task.isDone)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Instantiate all targets in the game world using their respective prefabs.
        /// </summary>
        protected void Spawn()
        {
            foreach (var target in Targets)
            {
                Transform prefab;
                switch (TargetType)
                {
                    case TargetType.Paper:
                        throw new NotImplementedException();
                    case TargetType.Dummy:
                        prefab = DummyTargetPrefab;
                        break;
                    case TargetType.Person:
                        prefab = GetRandomNpc();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                target.Spawn(prefab);

                if (target is TargetNpc)
                {
                    TargetNpc tnpc = ((TargetNpc)target);
                    tnpc.NPC.OnNPCDeathEvent += OnNpcDeath;
                    tnpc.NPC.OnNPCHitEvent += OnNpcHit;
                }
            }
        }

        /// <summary>
        /// Event triggered on the death of an NPC
        /// </summary>
        /// <param name="npc">NPC that died</param>
        /// <param name="hitmessage">info about the hit that killed the NPC</param>
        private void OnNpcDeath(NPC npc, HitMessage hitmessage)
        {
            if (hitmessage.IsPlayer)
            {
                StartCoroutine("gameoverWait", false);
            }
        }

        /// <summary>
        /// Even triggerd when an npc gets hit
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="hitmessage"></param>
        protected virtual void OnNpcHit(NPC npc, HitMessage hitmessage)
        {
            if (!npc.IsHostile && hitmessage.IsPlayer)
            {
                GameOver();
                ShowGameOverReason(StageEndReason.CivilianDied);
            }
        }

        /// <summary>
        /// Instantiates the waypoints of a target. The order of received vector3's 
        /// also decides the order of the waypoints. This is called by the Load script
        /// as the waypoints need to be instantiated anyway.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="positions"></param>
        public void SpawnWaypointsForTarget(Target target, List<Vector3> positions)
        {
            if (WaypointPrefab == null)
                throw new Exception("Unable to spawn Waypoint. There is no prefab set up.");

            foreach (Vector3 position in positions)
            {
                Waypoint waypoint = CreateWaypoint(target, position);
                waypoint.transform.parent = WaypointsObject;
                target.Waypoints.Add(waypoint);
            }
        }


        /// <summary>
        /// create waypoint at stated position
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public Waypoint CreateWaypoint(Target target, Vector3 position)
        {
            Transform t = Instantiate(WaypointPrefab, position, Quaternion.identity);
            Waypoint waypoint = t.GetComponent<Waypoint>();
            if (waypoint == null)
                throw new NullReferenceException("There is no Waypoint script in the waypoint prefab!");
            waypoint.Owner = target;
            return waypoint;
        }

        /// <summary>
        /// Returns a random NPC from the spawnable NPC list.
        /// </summary>
        /// <returns></returns>
        protected Transform GetRandomNpc()
        {
            if (PersonTargetPrefabs.Count == 0)
                throw new Exception("Unable to return random NPC. Please ensure the NPC list field is filled.");

            return PersonTargetPrefabs[RNG.Next(0, PersonTargetPrefabs.Count)];
        }

        /// <summary>
        /// Shows reason of gameover
        /// </summary>
        /// <param name="reason"></param>
        public void ShowGameOverReason(StageEndReason reason)
        {
            GameOverScreen.SetActive(true);

            switch (reason)
            {
                case StageEndReason.AgentDied:
                    GameOverScreenText.text = "Je bent geraakt.";
                    break;
                case StageEndReason.CivilianDied:
                    GameOverScreenText.text = "Je hebt een burger geraakt.";
                    break;
                default:
                    GameOverScreenText.text = "Game over";
                    break;
            }


        }

        /// <summary>
        /// Hides the game over message.
        /// </summary>
        public void HideGameOverReason()
        {
            GameOverScreen.SetActive(false);
        }


    }
}