using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This class fades in and out arrows which indicate to
    /// the player which direction they should be facing.
    /// This code is originally from the unity VR sample project.
    /// </summary>
    public class GUIArrows : MonoBehaviour
    {
        /// <summary>
        /// How long it takes for the arrows to appear and disappear.
        /// </summary>
        [SerializeField] private float m_FadeDuration = 0.5f;

        /// <summary>
        /// How far from the desired facing direction the player must be facing for the arrows to appear.
        /// </summary>
        [SerializeField] private float m_ShowAngle = 60f;

        /// <summary>
        /// Indicates which direction the player should be facing (uses world space forward if null).
        /// </summary>
        [SerializeField] private Transform m_DesiredDirection;

        /// <summary>
        /// Reference to the camera to determine which way the player is facing.
        /// </summary>
        [SerializeField] private Transform m_Camera;

        /// <summary>
        /// Reference to the renderers of the arrows used to fade them in and out.
        /// </summary>
        [SerializeField] private Renderer[] m_ArrowRenderers;

        /// <summary>
        /// The alpha the arrows currently have.
        /// </summary>
        private float m_CurrentAlpha;

        /// <summary>
        /// The alpha the arrows are fading towards.
        /// </summary>
        private float m_TargetAlpha;

        /// <summary>
        /// How much the alpha should change per second (calculated from the fade duration).
        /// </summary>
        private float m_FadeSpeed;

        private void Start()
        {
            // Speed is distance (zero alpha to one alpha) divided by time (duration).
            m_FadeSpeed = 1f / m_FadeDuration;
        }

        private void Update()
        {
            // The vector in which the player should be facing is the forward direction of the transform specified or world space.
            Vector3 desiredForward = m_DesiredDirection == null ? Vector3.forward : m_DesiredDirection.forward;

            // The forward vector of the camera as it would be on a flat plane.
            Vector3 flatCamForward = Vector3.ProjectOnPlane(m_Camera.forward, Vector3.up).normalized;

            // The difference angle between the desired facing and the current facing of the player.
            float angleDelta = Vector3.Angle(desiredForward, flatCamForward);

            // If the difference is greater than the angle at which the arrows are shown, their target alpha is one otherwise it is zero.
            m_TargetAlpha = angleDelta > m_ShowAngle ? 1f : 0f;

            // Increment the current alpha value towards the now chosen target alpha and the calculated speed.
            m_CurrentAlpha = Mathf.MoveTowards(m_CurrentAlpha, m_TargetAlpha, m_FadeSpeed * Time.deltaTime);

            // Go through all the arrow renderers and set the given property of their material to the current alpha.
            foreach (Renderer r in m_ArrowRenderers)
            {
                Color current = r.material.color;
                r.material.SetColor("_Color", new Color(current.r, current.g, current.b, m_CurrentAlpha));
            }
        }

        /// <summary>
        /// Turn off the arrows entirely.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Turn the arrows on.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}