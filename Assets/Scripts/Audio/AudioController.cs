using System;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using Assets.Scripts.NPCs;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    /// <summary>
    /// Used to play audio clips of specific categories. Uses volume levels.
    /// Can play audio clips at world positions or on GameObjects.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        private const float DEFAULT_GUN_VOLUME = 0.5f;
        private const float DEFAULT_DIALOGUE_VOLUME = 0.5f;

        public static float GunVolume { get; set; }
        public static float DialogueVolume { get; set; }

        public static bool IsLoaded;
        private static Dictionary<AudioCategory, List<AudioClip>> AudioDictionary;

        void Awake()
        {
            if (!IsLoaded) LoadAudio();
        }

        /// <summary>
        /// Fill dictionary with all known sounds types and their sound files.
        /// </summary>
        public static void LoadAudio()
        {
            IsLoaded = true;
            DialogueVolume = DEFAULT_DIALOGUE_VOLUME;

            AudioDictionary = new Dictionary<AudioCategory, List<AudioClip>>();

            // Bird
            AddToDict(AudioCategory.BirdChirp, "Bird/Chirp");
            AddToDict(AudioCategory.BirdDeath, "Bird/Death");
            AddToDict(AudioCategory.BirdWingFlap, "Bird/Wing-Flap");

            // Gun
            AddToDict(AudioCategory.GunDraw, "Gun/Draw");
            AddToDict(AudioCategory.GunHitMetal, "Gun/Hit/Metal");
            AddToDict(AudioCategory.GunHitStone, "Gun/Hit/Stone");
            AddToDict(AudioCategory.GunHitWood, "Gun/Hit/Wood");
            AddToDict(AudioCategory.GunShoot, "Gun/Shoot");
            AddToDict(AudioCategory.GunPoliceShoot, "Gun/PoliceShoot");
            AddToDict(AudioCategory.GunTrigger, "Gun/Trigger");
            AddToDict(AudioCategory.GunShoot2, "Gun/Shoot2");
            AddToDict(AudioCategory.GunAk47, "Gun/AK47");

            // Pain
            AddToDict(AudioCategory.PainFemaleLethal, "Pain/Female/Lethal");
            AddToDict(AudioCategory.PainFemaleNonLethal, "Pain/Female/Non-Lethal");
            AddToDict(AudioCategory.PainMaleLethal, "Pain/Male/Lethal");
            AddToDict(AudioCategory.PainMaleNonLethal, "Pain/Male/Non-Lethal");


            // Scream
            AddToDict(AudioCategory.ScreamFemale, "Scream/Female");
            AddToDict(AudioCategory.ScreamGroup, "Scream/Group");
            AddToDict(AudioCategory.ScreamMale, "Scream/Male");

            // Rest
            AddToDict(AudioCategory.Agrociv, "Agro-civ");
            AddToDict(AudioCategory.Agroplayer, "Agro-player");
            AddToDict(AudioCategory.Ambience, "Ambience");
            AddToDict(AudioCategory.Explosion, "Explosion");
            AddToDict(AudioCategory.Footstep, "Footstep");
            AddToDict(AudioCategory.Menu, "Menu");
            AddToDict(AudioCategory.Surrender, "Surrender");
            AddToDict(AudioCategory.DangerZone, "DangerZone");
        }

        /// <summary>
        /// Create a struct from Soundfiles
        /// </summary>
        /// <param name="Enum">Type name in Dictionary</param>
        /// <param name="folderLocation">Location of the soundfiles folder</param>
        private static void AddToDict(AudioCategory Enum, string folderLocation)
        {
            AudioClip[] clipsArray = Resources.LoadAll<AudioClip>("Sounds/" + folderLocation);

            List<AudioClip> clips = new List<AudioClip>();
            if (clipsArray != null)
            {
                clips.AddRange(clipsArray);
            }
            else
            {
                Debug.LogWarning(string.Format("Tried loading sound files at Resources/Sounds/{0} but found nothing.",
                    folderLocation));
            }

            AudioDictionary.Add(Enum, clips);
        }

        /// <summary>
        /// Play requested audio on GameObject. Adds AudioSource if necessary.
        /// </summary>
        /// <param name="obj">Gameobject that plays the sound</param>
        /// <param name="type">Type of sound to be made</param>
        /// <param name="volume">Range: [0,1]</param>
        /// <param name="loop"></param>
        public static void PlayAudio(GameObject obj, AudioCategory type, float volume = 1.0f, bool loop = false)
        {
            AudioSource source = obj.GetComponent<AudioSource>();
            if (source == null)
            {
                // Create AudioSource if necessary and set default values.
                source = obj.AddComponent<AudioSource>();
                source.spatialBlend = 1;
                source.spread = 90;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.playOnAwake = false;
            }

            // Select random clip
            List<AudioClip> audioClips = AudioDictionary[type]; // All audio files of this type
            int number = RNG.Next(0, audioClips.Count);
            AudioClip audioClip = audioClips[number]; // Loads the audioclip from the dictionary

            if (audioClip != null)
            {
                source.clip = audioClip;
                source.loop = loop;
                if (type.ToString().ToLower().Contains("pain") || type.ToString().ToLower().Contains("scream")) {
                    source.volume = 0.2f;
                }
                source.Play();
            }
            else
            {
                throw new IndexOutOfRangeException(
                    "Unable to select AudioClip. Ensure that lists in AudioDictionary are filled.");
            }
        }

        /// <summary>
        /// Play requested audio at a location in the game world.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="type"></param>
        /// <param name="volume">range [0,1]</param>
        public static void PlayAudioAtPoint(Vector3 location, AudioCategory type, float volume = 1.0f)
        {
            List<AudioClip> audioClips = AudioDictionary[type]; // All audio files of this type

            // Play random clip from array
            int number = RNG.Next(0, audioClips.Count);
            AudioClip audioClip = audioClips[number]; // Loads the audioclip from the dictionary

            if (audioClip != null)
            {
                AudioSource.PlayClipAtPoint(audioClip, location, volume);
            }
            else
            {
                throw new IndexOutOfRangeException(
                    "Unable to select AudioClip. Ensure that lists in AudioDictionary are filled.");
            }
        }

        /// <summary>
        /// Play a scream sound clip. Checks gender/body type for correct sound.
        /// </summary>
        /// <param name="npc"></param>
        public static void ScreamAudio(NPC npc)
        {
            AudioCategory audioCatToPlay;
            switch (npc.BodyType)
            {
                case BodyType.Boy:
                case BodyType.Man:
                    audioCatToPlay = AudioCategory.ScreamMale;
                    break;
                case BodyType.Girl:
                case BodyType.Woman:
                    audioCatToPlay = AudioCategory.ScreamFemale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PlayAudio(npc.gameObject, audioCatToPlay);
        }

        /// <summary>
        /// Play pain sound clip. Checks gender/body type for correct sound.
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="isLethal"></param>
        public static void PainAudio(NPC npc, bool isLethal)
        {
            AudioCategory audioCatToPlay;
            switch (npc.BodyType)
            {
                case BodyType.Boy:
                case BodyType.Man:
                    audioCatToPlay = isLethal ? AudioCategory.PainMaleLethal : AudioCategory.PainMaleNonLethal;
                    break;
                case BodyType.Girl:
                case BodyType.Woman:
                    audioCatToPlay = isLethal ? AudioCategory.PainFemaleLethal : AudioCategory.PainFemaleNonLethal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PlayAudio(npc.gameObject, audioCatToPlay);
        }
    }
}