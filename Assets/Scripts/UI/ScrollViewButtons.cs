using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Enables the use of buttons to change the scroll value of a ScrollView.
    /// Placed on a Scrollbar GameObject.
    /// </summary>
    public class ScrollViewButtons : MonoBehaviour
    {
        [SerializeField] private float _stepping = DEFAULT_STEPPING;
        [SerializeField] private Scrollbar _scrollbar;

        private const float DEFAULT_STEPPING = 0.1f;

        private bool _initialized;

        private void Start()
        {
            if (!_scrollbar)
                throw new NullReferenceException(
                    "Unable to initialize scroll buttons, ensure " +
                    "ScrollViewButtons has a reference to the Scrollbar.");
            _initialized = true;
        }

        /// <summary>
        /// Called by a button to scroll the scrollbar up.
        /// </summary>
        public void Up()
        {
            if (_initialized)
                _scrollbar.value = Mathf.Clamp01(_scrollbar.value + _stepping);
        }

        /// <summary>
        /// Called by a button to scroll the scrollbar down.
        /// </summary>
        public void Down()
        {
            if (_initialized)
                _scrollbar.value = Mathf.Clamp01(_scrollbar.value - _stepping);
        }
    }
}