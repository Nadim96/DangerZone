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
        private const string PLEIN_SCENE_NAME = "Street";
        private const string SCENARIO_SCENE_NAME = "Bunker";
        private const string DOOR_SCENE_NAME = "GenerateRoomTest";

        void Awake()
        {
            AudioController.LoadAudio();
        }

        public enum SceneToLoad
        {
            Plein,
            Scenario,
            Door
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
        public void LoadPlein(float delay)
        {
            StartCoroutine(LoadSceneCoroutine(SceneToLoad.Plein, delay));
        }

        public void LoadScenario(float delay)
        {
            StartCoroutine(LoadSceneCoroutine(SceneToLoad.Scenario, delay));
        }

        public void LoadRandom(float delay)
        {
            ScenarioSettings.IsRandomScenario = true;
            StartCoroutine(LoadSceneCoroutine(SceneToLoad.Scenario, delay));
        }

        public void LoadDoor(float delay)
        {
            ScenarioSettings.IsRandomScenario = true;
            StartCoroutine(LoadSceneCoroutine(SceneToLoad.Door, delay));
        }

        private static IEnumerator LoadSceneCoroutine(SceneToLoad scene, float delay)
        {
            string sceneName;
            switch (scene)
            {
                case SceneToLoad.Plein:
                    sceneName = PLEIN_SCENE_NAME;
                    break;
                case SceneToLoad.Scenario:
                    sceneName = SCENARIO_SCENE_NAME;
                    break;
                case SceneToLoad.Door:
                    sceneName = DOOR_SCENE_NAME;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("scene", scene, null);
            }

            yield return new WaitForSeconds(delay);

            var task = SceneManager.LoadSceneAsync(sceneName);

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