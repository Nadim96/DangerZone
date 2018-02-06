using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    public class GameOver : MonoBehaviour
    {
        public Color levelLose = Color.red;
        public Color levelWin = Color.green;

        public List<Light> lights;

        public static GameOver instance;

        public GameObject scenarioStarted;
        public GameObject scenarioNotStarted;

        public GameOver()
        {
            instance = this;
        }

        /// <summary>
        /// Set gameoverscreen.
        /// </summary>
        /// <param name="won"></param>
        public void SetEndscreen(bool won)
        {
            //set color
            foreach (Light light1 in lights)
                light1.color = won ? levelWin : levelLose;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide gameover
        /// </summary>
        public void HideEndScreen()
        {
            gameObject.SetActive(false);
        }
    }
}