using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// VR-compatible numeric input field. 
    /// Supports keyboard, mouse and laserpointer.
    /// </summary>
    public class NumericUpDown : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _textField;
        [SerializeField] private int _minValue = DEFAULT_MIN_VALUE;
        [SerializeField] private int _maxValue = DEFAULT_MAX_VALUE;

        /// <summary>
        /// Value cannot be set lower then the value of this class
        /// can be left null
        /// </summary>
        [SerializeField] private NumericUpDown _lowerBounds;

        /// <summary>
        /// Value cannot be set higher then the value of this class
        /// can be left null
        /// </summary>
        [SerializeField] private NumericUpDown _upperBounds;

        public event Action<int> OnValueChanged;

        private const int DEFAULT_MIN_VALUE = 1;
        private const int DEFAULT_MAX_VALUE = 99;

        private int _restoreValue;

        private void Awake()
        {
            _textField.onSelect.AddListener(OnSelect);
            _textField.onEndEdit.AddListener(OnEndEdit);
        }

        /// <summary>
        /// Stores the current value of the textfield 
        /// in case we need to restore it.
        /// </summary>
        /// <param name="value"></param>
        private void OnSelect(string value)
        {
            _restoreValue = int.Parse(value);
        }

        /// <summary>
        /// User is finished. Validate input and restore if necessary.
        /// </summary>
        /// <param name="value"></param>
        private void OnEndEdit(string value)
        {
            if (IsValid(value))
            {
                if (OnValueChanged != null)
                    OnValueChanged(Value);
            }
            else
            {
                Value = _restoreValue;
            }
        }

        /// <summary>
        /// Get value from textfield and convert it to int
        /// </summary>
        public int Value
        {
            get
            {
                int result;
                if (!int.TryParse(_textField.text, out result))
                    throw new ArgumentException("Number expected");

                return result;
            }
            set
            {
                if (OnValueChanged != null)
                    OnValueChanged(value);

                _textField.text = value.ToString();
            }
        }

        /// <summary>
        /// increase the Value
        /// </summary>
        /// <remarks>If a upperbound script is added,
        /// the vlaue cant be higher</remarks>
        public void Up()
        {
            if (Value >= _maxValue) return;

            if (_upperBounds != null
                && Value >= _upperBounds.Value)
            {
                _upperBounds.Value++;
                ;
            }

            Value++;

            if (OnValueChanged != null)
                OnValueChanged(Value);
        }


        /// <summary>
        /// decrease the Value
        /// </summary>
        /// <remarks>If a lowerbound is set. the value cannot be lower then it</remarks>
        public void Down()
        {
            if (Value <= _minValue) return;

            if (_lowerBounds != null
                && Value <= _lowerBounds.Value)
            {
                _lowerBounds.Value--;
            }

            Value--;

            if (OnValueChanged != null)
                OnValueChanged(Value);
        }

        /// <summary>
        /// Checks if a string is a valid number or corrects it to the nearest valid number
        /// </summary>
        /// <param name="value">string to check</param>
        private bool IsValid(string value)
        {
            int x;

            if (!int.TryParse(value, out x))
                return false;

            if (_lowerBounds != null && x < _lowerBounds.Value)
            {
                Value = _lowerBounds.Value;
                return true;
            }

            if (_upperBounds != null && x > _upperBounds.Value)
            {
                Value = _upperBounds.Value;
                return true;
            }

            if (x < _minValue)
            {
                Value = _minValue;
                return true;
            }

            if (x > _maxValue)
            {
                Value = _maxValue;
            }
            return true;
        }
    }
}