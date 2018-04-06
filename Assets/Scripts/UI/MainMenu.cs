using System;
using System.Collections;
using Assets.Scripts.Audio;
using Assets.Scripts.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Responsible for loading other scenes from the main menu scene 
    /// and quitting the game from the main menu. 
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        void Awake()
        {
            AudioController.LoadAudio();
        }

        public enum SceneToLoad
        {
            Plein,
            Bunker,
            Door,
            Street,
            StreetTutorial,
            None
        }

        private readonly KeyCode[] _konamiCode = {
            KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow,
            KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A
        };
        private int _konamiProgress = 0;
        private bool pressed;
        void Update()
        {
            #region konami
            if (Input.GetKeyDown(_konamiCode[_konamiProgress]))
            {
                Debug.Log(_konamiCode[_konamiProgress]);
                _konamiProgress++;
                if (_konamiProgress >= _konamiCode.Length)
                {
                    //success
                    _konamiProgress = 0;
                    Debug.Log("konami");
                    AudioController.PlayAudioAtPoint(Vector3.zero, AudioCategory.DangerZone);
                }
            }
            else if (Input.anyKeyDown)
                _konamiProgress = 0;
            #endregion

        }
        /// <summary>
        /// Funciton to translate the string to a scene
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        private SceneToLoad stringToScene(string myString)
        { 
            SceneToLoad returnScene = SceneToLoad.None;

            switch (myString)
            {
                case "Plein":
                    returnScene = SceneToLoad.Plein;
                    break;
                case "Bunker":
                    returnScene = SceneToLoad.Bunker;
                    break;
                case "Door":
                    returnScene = SceneToLoad.Door;
                    break;
                case "Street":
                    returnScene = SceneToLoad.Street;
                    break;
                case "StreetTutorial":
                    returnScene = SceneToLoad.StreetTutorial;
                    break;
             }

            return returnScene;
        }

        /// <summary>
        /// Loads a level
        /// </summary>
        /// <param name="level"></param>
        public void LoadLevel(string level)
        {
            //ScenarioSettings.IsRandomScenario = (level == SceneToLoad.Door.ToString());
            StartCoroutine(LoadSceneCoroutine(stringToScene(level)));
        }
    
        private static IEnumerator LoadSceneCoroutine(SceneToLoad scene)
        {
          yield return new WaitForSeconds(1);

            var task = SceneManager.LoadSceneAsync(scene.ToString());

            while (!task.isDone)
            {
                yield return null;
            }
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}