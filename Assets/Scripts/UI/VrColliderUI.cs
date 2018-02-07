using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This class is used on canvas elements. It creates a dynamically 
    /// sized box-collider for use with the VR UI interaction system. 
    /// A Boxcollider is necessary to trigger laser pointer events.
    /// This script avoids having the user resize the collider each 
    /// time they change a button.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class VrColliderUI : MonoBehaviour
    {
        private BoxCollider _boxCollider;
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            ValidateCollider();
        }

        private void OnValidate()
        {
            ValidateCollider();
        }

        private void ValidateCollider()
        {
            _rectTransform = GetComponent<RectTransform>();

            _boxCollider = GetComponent<BoxCollider>();
            if (!_boxCollider) _boxCollider = gameObject.AddComponent<BoxCollider>();

            _boxCollider.size = _rectTransform.sizeDelta;
        }
    }
}