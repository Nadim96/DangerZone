using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Class used to create VR-compatible buttons which can be used to select one 
    /// option out of multiple. When using this dropdown with enums, ensure the 
    /// button events in the inspector matches the order of the enum definition.
    /// </summary>
    public class Dropdown : MonoBehaviour
    {
        [SerializeField] private Color _deselectedColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private List<Button> _buttons;
        [SerializeField] private Button _defaultSelection;

        public event Action<int> OnSelectedIndexChanged;

        public int Value { get; set; }

        private void Awake()
        {
            int i = _buttons.FindIndex(x => x == _defaultSelection);
            Select(i);
        }

        /// <summary>
        /// Used by Unity button click events to select a specific item in the dropdown.
        /// </summary>
        /// <param name="index">index used by button</param>
        /// <remarks>
        /// The index used in Unity Must match with the value of the enum used when parsing the index
        /// </remarks>
        public void Select(int index)
        {
            if (index < 0 || index >= _buttons.Count)
                throw new ArgumentOutOfRangeException("index");

            Value = index;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].image.color = i == index ? _selectedColor : _deselectedColor;
            }

            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged(index);
        }
    }
}