using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// This fade effect is used for fading effects on the monitor.
    /// VR-set uses its own SteamVR_Fade class for this.
    /// </summary>
    public class FadeEffect : MonoBehaviour
    {
        /// <summary>
        /// If true, screen will fade in on awake.
        /// </summary>
        public bool BeginDark;

        public float FadeDuration = 2f;

        private float _currentAlpha;

        /// <summary>
        /// shader used
        /// </summary>
        /// <remarks>
        /// This can't be done dynamicly with shader.find() because the shader does not exist in build.
        /// </remarks>
        [SerializeField] private Material _fadeMaterial;

        private void Awake()
        {
            if (BeginDark)
            {
                _currentAlpha = 1f;
                FadeIn();
            }
        }

        void OnPostRender()
        {
            if (Math.Abs(_currentAlpha) < 0.0001f) return;

            _fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(_fadeMaterial.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
            
        }

        /// <summary>
        /// Starts FadeTo coroutine. a=1 -> a=0
        /// </summary>
        public void FadeIn(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(FadeTo(0f, duration));
        }

        public void FadeIn()
        {
            FadeIn(FadeDuration);
        }

        /// <summary>
        /// Starts FadeTo coroutine. a=0 -> a=1
        /// </summary>
        public void FadeOut(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(FadeTo(1f, duration));
        }

        public void FadeOut()
        {
            FadeOut(FadeDuration);
        }

        /// <summary>
        /// Coroutine for fading. Updates the alpha value of the material without stopping the game.
        /// 0 means vision is not impeded. 255 means vision is completely blocked.
        /// </summary>
        /// <param name="alpha">Alpha value to fade to. Alpha value must be within the [0, 255] range.</param>
        /// <param name="duration">The time it should take to fade to the specified value.</param>
        /// <returns></returns>
        IEnumerator FadeTo(float alpha, float duration)
        {
            if (alpha < 0 || alpha > 1)
                throw new ArgumentOutOfRangeException("alpha", "Alpha value must be within the [0, 255] range.");

            var c = _fadeMaterial.color;
            //this probably should be unscaleddeltatime to prevent timescale 0 when gameover from messing with it
            //but im to afraid to change it this late into the project.
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) 
            {
                _currentAlpha = Mathf.Lerp(_currentAlpha, alpha, t);
                _fadeMaterial.color = new Color(c.r, c.g, c.b, _currentAlpha);
                yield return null;
            }
        }
    }
}