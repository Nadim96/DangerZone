using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Assets.Scripts.NPCs;
using Assets.Scripts.Items;
using Assets.Scripts.Utility;
using Assets.Scripts.Settings;
using Assets.Scripts.BehaviourTree;

namespace Assets.Scripts.Scenario
{
    public class TutorialScenario : ScenarioBase
    {
        /// <summary>
        /// The stages that are in the tutorial
        /// </summary>
        private enum Stage
        {
            None,
            ShowWorld,
            Movement,
            GoalExplention,
            Cover,
            Practise,
        }

        private Dictionary<string, string> Messages = new Dictionary<string, string>()
        {
            {
                "Welcome",
                "WELKOM IN DANGER ZONE." + '\n' + '\n' +
                "DIT IS EEN VAARDIGHEIDSTRAINING IN EEN RIJKE CONTEXT."
            },
            {
                "Movement",'\n' +
                "DE BLAUWE LIJNEN OP DE GROND GEVEN HET SPEELVELD AAN. HIERIN KUN JE RONDLOPEN ZONDER IN HET ECHT ERGENS TEGENAAN TE LOPEN." +'\n' + '\n' + '\n' +
                "LOOP DOOR DE RUIMTE OM VERDER TE GAAN."
            },
            {
                "Goal",
                "JE DOEL IS OM VERDACHTEN TE NEUTRALISEREN EN HIERBIJ GEEN BURGERS AAN TE WIJZEN OF TE RAKEN. " + 
                "JE HEBT 15 KOGELS OM DIT DOEL TE BEREIKEN." +'\n' + '\n' +"NEUTRALISEER DE VERDACHTE OM VERDER TE GAAN."
            },
            {
                "Cover",
                "OM HET SPEL TE WINNEN MAG JE ZELF NIET GERAAKT WORDEN. DIT KUN JE DOEN DOOR GEBRUIK TE MAKEN VAN DEKKING ACHTER VERSCHILLENDE OBJECTEN IN HET SPEL."
            },
            {
                "Practise",
                "KLAAR OM TE OEFENEN?" + '\n' + '\n' +
                "SCHIET OP 'START' OM TE BEGINNEN."
            },
            {
                "PractiseSucces",
                "GOED GEDAAN! " +'\n' + '\n' +
                "SCHIET OP 'STOP' ONDER JE VOETEN OM TERUG TE GAAN NAAR HET HOOFDMENU, OF DRUK OP RESTART OM OPNIEUW TE OEFENEN."
            },
            {
                "PractiseAgent",
                "HELAAS, JE BENT NEERGESCHOTEN. " + '\n' + '\n' +
                "SCHIET OP 'RESTART' ONDER JE VOETEN OM HET OPNIEUW TE PROBEREN."
            },
            {
                "PractiseCiv",
                "HELAAS, JE HEBT EEN BURGER NEERGESCHOTEN." + '\n' + '\n' +     
                "SCHIET OP 'RESTART' ONDER JE VOETEN OM HET OPNIEUW TE PROBEREN."
            }

        };

        // Current stage
        private Stage CurrentStage = Stage.None;
        private Difficulty difficulty = Difficulty.None;

        // Points for spawning of npcs
        public Transform[] NPCSpawnPoints;

        public GameObject UIRoot;

        // Ingame menu and dialog screen
        public GameObject IngameMenu;
        public Text IngameMenuText;
        public Text IngameMenuStarText;
        public GameObject IngameMenuStartButton;
        public GameObject StartButton;
        public GameObject[] CoverBodies;

        private float timer;
        private Vector3 startingPosition;
        private bool HasLostBefore = false;

        protected override void Load()
        {
            SetDifficulty(Difficulty.None);
            base.Load();
        }

        protected override void Start()
        {
            base.Start();
            SetMenuEnabled(true);
            timer = 0;
        }

        protected override void Update()
        {
            UpdateStage(CurrentStage);

            if (Started && !AttackTriggered)
            {
                if (Time.time > ScenarioStartedTime + timeBeforeAttack)
                {
                    AttackTriggered = true;
                }
            }
        }

        /// <summary>
        /// Gets triggered when the ingame menu's "Play" button is activated
        /// </summary>
        public void OnMenuPlayButton()
        {
            Debug.Log("Button Pressed");
            PlayerGun.PlayerGunInterface.ReloadGun();
            switch (CurrentStage) {
                case Stage.Practise:
                    Debug.Log("Practice MODE");
                    SetMenuEnabled(false);
                    SetLoadType();
                    Scenario.GameOver.instance.HideEndScreen();
                    SetDifficulty(Difficulty.Street);
                    Started = true;
                    AttackTriggered = false;
                    ScenarioStartedTime = Time.time;
                    timeBeforeAttack = RNG.NextFloat(minTimeElapsedBeforeAttack, maxTimeElapsedBeforeAttack);
                    //if (!HasLostBefore)
                    {
                        ClearNPCS();
                        Time.timeScale = 1f;
                        base.Load();
                        base.Create();
                        base.Spawn();
                    }
                    break;
                case Stage.GoalExplention:
                    SetMenuEnabled(false);
                    break;
                 default :
                    Play();
                    break;
            }
        }

        protected override void OnNpcHit(NPC npc, HitMessage hitmessage)
        {
            if (!npc.IsHostile && hitmessage.IsPlayer)
            {
                EndStage(CurrentStage, StageEndReason.CivilianDied);
            }
        }

        /// <summary>
        /// Gets triggered when the restart button on the ground is activated and restarts the stage
        /// </summary>
        public void OnRestartButton()
        {
            PlayerGun.PlayerGunInterface.ReloadGun();
            Play();
            Scenario.GameOver.instance.HideEndScreen();
            SetMenuEnabled(false);
            base.Load();
            base.Create();
            base.Spawn();
        }

        /// <summary>
        /// Toggles the UI on the ground
        /// </summary>
        public override void SetIngameUIVisible()
        {
            EnableIngameMenu = true;
            IngameUI.SetActive(true);
        }

        /// <summary>
        /// Starts the next stage
        /// </summary>
        public override void Play()      
        {
            // Increments to the next stage if not practise
            if (CurrentStage != Stage.Practise)
            {
                CurrentStage++;
            }

            // Sets the stage
            ClearNPCS();
            Started = true;
            Time.timeScale = 1f;
            SetMenuEnabled(false);
            AttackTriggered = false;
            ScenarioStartedTime = Time.time;
            PlayerCameraEye.GetComponent<Player.Player>().Health = 100;
            timeBeforeAttack = RNG.NextFloat(minTimeElapsedBeforeAttack, maxTimeElapsedBeforeAttack);

            if (!HasLostBefore)
            {
                StartStage(CurrentStage);
            }
        }

        /// <summary>
        /// Clears all npcs of the map
        /// </summary>
        private void ClearNPCS()
        {
            foreach (Target t in Targets)
            {
                t.Destroy();
            }
            Targets.Clear();
            NPC.Npcs.Clear();
            NPC.HostileNpcs.Clear();
        }

        /// <summary>
        /// Sets the text on the ingame menu
        /// </summary>
        /// <param name="text"></param>
        /// <param name="aditionalinfo"></param>
        private void SetMenuText(string text)
        {
            IngameMenuText.text = text;
        }

        /// <summary>
        /// Enables or disables the ingame menu
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMenuEnabled(bool enabled)
        {
            IngameMenu.SetActive(enabled);
            EnableIngameMenu = enabled;
        }

        /// <summary>
        /// Gets wheter the ingamemenu is active
        /// </summary>
        public bool IsMenuEnabled { get { return IngameMenu.activeSelf; } }

        /// <summary>
        /// Enables the menu start button
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMenuStart(bool enabled)
        {
            IngameMenuStartButton.SetActive(enabled);
        }

        public void SetMenuStartText(string text)
        {
            IngameMenuStarText.text = text;
        }

        /// <summary>
        /// Stops the scenario 
        /// </summary>
        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }

        /// <summary>
        /// Triggers game over state
        /// </summary>
        public override void GameOver()
        {
            EndStage(CurrentStage, StageEndReason.AgentDied);
            Scenario.GameOver.instance.SetEndscreen(false);
            Time.timeScale = 0.0f;
            //SetMenuEnabled(true);
            SetIngameUIVisible();
        }

        /// <summary>
        /// Starts a stage
        /// </summary>
        /// <param name="stage">the stage to be started</param>
        private void StartStage(Stage stage)
        {


            Scenario.GameOver.instance.HideEndScreen();
            //Handles every stage
            switch (stage)
            {
                case Stage.None: break;
                case Stage.ShowWorld:
                    string m = "";
                    Messages.TryGetValue("Welcome", out m);
                    SetMenuText(m);

                    SetMenuStart(true);
                    SetMenuEnabled(true);
                    SetMenuStartText("START");
                    break;
                case Stage.Movement:
                    string s = "";
                    Messages.TryGetValue("Movement", out s);
                    SetMenuText(s);

                    SetMenuStart(false);
                    SetMenuEnabled(true);
                    Vector3 loc = this.PlayerCameraEye.transform.position;
                    startingPosition = new Vector3(loc.x, loc.y, loc.z);
                    break;
                case Stage.GoalExplention:
                    string x = "";
                    Messages.TryGetValue("Goal", out x);
                    SetMenuText(x);

                    SetMenuEnabled(true);
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SetMenuStart(true);
                    break;
                case Stage.Cover:
                    string z = "";
                    Messages.TryGetValue("Cover", out z);
                    SetMenuText(z);
                    SetMenuEnabled(true);
                    EnableCoverBodies(true);
                    SetMenuStartText("VERDER");
                    SetMenuStart(false);
                    timer = 0;
                    break;
                case Stage.Practise:
                    string c = "";
                    Messages.TryGetValue("Practise", out c);
                    SetMenuText(c);

                    EnableCoverBodies(false);
                    StartButton.SetActive(true);
                    if (!HasLostBefore)
                    {
                        SetMenuEnabled(true);
                    }
                    SetMenuStartText("START");
                    break;
            }
        }

        /// <summary>
        /// Updates during the stage
        /// </summary>
        /// <param name="stage">the current stage being updated</param>
        private void UpdateStage(Stage stage)
        {
            timer += Time.deltaTime;

            switch (stage)
            {
                case Stage.None: break;
                case Stage.ShowWorld:
                    break;
                case Stage.Movement:
                    Vector3 distance = this.PlayerCameraEye.transform.position - startingPosition;
                    if (distance.magnitude > 1)
                    {
                        EndStage(CurrentStage, StageEndReason.Succes);
                    }
                    break;
                case Stage.GoalExplention:
                    if (NPC.HostileNpcs.Count == 0)
                    {
                        EndStage(CurrentStage, StageEndReason.Succes);
                        SetMenuStart(true);
                    }
                    break;
                case Stage.Cover:
                    if (timer < 5)
                    {
                        SetMenuStart(false);
                    }
                    else
                    {
                        SetMenuStart(true);
                    }
                    break;
                case Stage.Practise:
                    if (NPC.HostileNpcs.Count == 0 && !IsMenuEnabled)
                    {
                        EndStage(CurrentStage, StageEndReason.Succes);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles the ending of a stage
        /// </summary>
        /// <param name="stage">stage to be ended</param>
        /// <param name="reason">reason why the stage ended</param>
        private void EndStage(Stage stage, StageEndReason reason)
        {
            Time.timeScale = 0f;
            Started = false;

            // Handle every case
            switch (stage)
            {
                case Stage.None: break;
                case Stage.ShowWorld:
                    Play();
                    break;
                case Stage.Movement:
                    Play();
                    break;
                case Stage.GoalExplention:
                    SetMenuEnabled(false);
                    Play();
                    break;
                case Stage.Cover:
                    EnableCoverBodies(false);
                    break;
                case Stage.Practise:
                    UIRootFloor.SetActive(true);
                    switch (reason)
                    {
                        case StageEndReason.Succes:
                            string c = "";
                            Messages.TryGetValue("PractiseSucces", out c);
                            SetMenuText(c);

                            Scenario.GameOver.instance.SetEndscreen(true);
                            SetMenuStart(false);
                            SetMenuEnabled(true);
                            break;
                        case StageEndReason.AgentDied:
                            string d = "";
                            Messages.TryGetValue("PractiseAgent", out d);
                            SetMenuText(d);

                            HasLostBefore = true;
                            SetMenuEnabled(true);
                            Scenario.GameOver.instance.SetEndscreen(false);
                            break;
                        case StageEndReason.CivilianDied:
                            string x = "";
                            Messages.TryGetValue("PractiseCiv", out x);
                            SetMenuText(x);

                            HasLostBefore = true;
                                SetMenuEnabled(true);
                            Scenario.GameOver.instance.SetEndscreen(false);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Toggles the bodies in the scene
        /// </summary>
        /// <param name="enable"></param>
        private void EnableCoverBodies(bool enable)
        {
            foreach (GameObject obj in CoverBodies)
            {
                obj.SetActive(enable);
            }
        }

        /// <summary>
        /// Spawn an npc
        /// </summary>
        /// <param name="hostile">wether the npc is hostile</param>
        /// <param name="type">what kind of npc is to be spawned</param>
        /// <param name="location">location of the npc</param>
        /// <param name="rotation">rotation of the npc</param>
        private void SpawnNPC(bool hostile, Transform type, Vector3 location, Quaternion rotation)
        {
            // Create npc
            TargetNpc t = new TargetNpc();
            t.Difficulty = difficulty;
            t.ItemType = ItemType.P99;
            t.Position = location;
            t.IsHostile = hostile;

            Targets.Add(t);

            // Spawn npc
            Transform trans = t.Spawn(type);

            t.NPC.OnNPCDeathEvent += OnNPCDeath;
            trans.rotation = rotation;
        }

        /// <summary>
        /// Event triggered when a npc dies
        /// </summary>
        /// <param name="npc">the npc that died</param>
        /// <param name="hitmessage">the info about the hit</param>
        private void OnNPCDeath(NPC npc, HitMessage hitmessage)
        {
            timer = 0;
            // Check if the npc was killed by a player and was a neutral npc
            if (!npc.IsHostile && hitmessage.IsPlayer)
            {
                // End the current stage
                EndStage(CurrentStage, StageEndReason.CivilianDied);
            }
        }

        /// <summary>
        /// Sets the current diffeculty
        /// </summary>
        /// <param name="difficulty">the difficulty</param>
        private void SetDifficulty(Difficulty difficulty)
        {
            this.difficulty = difficulty;
            LoadStyle.SetDifficulty(difficulty);
        }

        /// <summary>
        /// Check to see if a stage ended
        /// </summary>
        /// <returns>whether the stage has ended</returns>
        private bool StageEnded()
        {
            return Started && NPC.HostileNpcs.Count == 0;
        }

    }
}
