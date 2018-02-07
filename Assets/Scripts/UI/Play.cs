using System;
using System.Collections.Generic;
using Assets.Scripts.Scenario;
using Assets.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Script used in the Play Scenario screen.
    /// Handles the scenario list and loads the correct scenario.
    /// </summary>
    public class Play : MonoBehaviour
    {
        /// <summary>
        /// Reference to the MainMenu, used to load the scenario scene.
        /// </summary>
        [SerializeField] private MainMenu _mainMenu;

        /// <summary>
        /// Content container of the Scroll View.
        /// </summary>
        [SerializeField] private GameObject _content;

        /// <summary>
        /// Prefab to use when populating the list.
        /// </summary>
        [SerializeField] private GameObject _listItemPrefab;

        /// <summary>
        /// ScrollView of this window. Will be disabled when
        /// scenario list is empty.
        /// </summary>
        [SerializeField] private GameObject _scrollView;

        /// <summary>
        /// Object containing a textual notice informing the user
        /// that the scenario list is empty.
        /// </summary>
        [SerializeField] private GameObject _listEmptyNotice;

        /// <summary>
        /// Height of a ListItem. Needed to correctly place ListItems.
        /// </summary>
        [SerializeField] private int _listItemHeight;

        /// <summary>
        /// Determines the spacing between ListItems.
        /// </summary>
        [SerializeField] private int _listItemMargin;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _randomPlayButton;

        /// <summary>
        /// Time the game should wait before starting to load. 
        /// This ensures fadeout effects can properly play.
        /// </summary>
        private const float PLAY_CLICK_LOAD_DELAY = 1f;

        /// <summary>
        /// The index of the ListItem which is currently selected.
        /// </summary>
        private int _selectedIndex;

        private List<string> _scenarioNames;
        private List<string> _scenarioPaths;

        private void Start()
        {
            if (!_content)
            {
                throw new NullReferenceException(
                    "No reference to Content. Ensure ScenarioListView has a " +
                    "reference to the Scroll View Content GameObject.");
            }

            if (!_listItemPrefab)
            {
                throw new NullReferenceException(
                    "Unable to fill list. No prefab ListItem to instantiate. " +
                    "Ensure ScenarioListView has a reference to a ListItem prefab.");
            }

            if (PopulateList())
            {
                if (_playButton) _playButton.interactable = true;
                if (_randomPlayButton) _randomPlayButton.interactable = true;
            }
            else
            {
                _listEmptyNotice.SetActive(true);
                _scrollView.SetActive(false);
            }
        }

        /// <summary>
        /// Gets currently selected scenario and set it as the scenario to load
        /// by the Scenario.Load class. The caller should also load the correct scene.
        /// </summary>
        public void PlayScenario()
        {
            if (_selectedIndex < 0 || _scenarioNames == null ||
                _selectedIndex >= _scenarioNames.Count)
            {
                throw new InvalidOperationException(
                    "There is no selected scenario or it is invalid.");
            }

            LoadXml.ScenarioToLoad = _scenarioPaths[_selectedIndex];
            if (!_mainMenu)
            {
                throw new NullReferenceException(
                    "ScenarioListView has no reference to the MainMenu script.");
            }
            _mainMenu.LoadScenario(PLAY_CLICK_LOAD_DELAY);
        }

        /// <summary>
        /// Selects a random scenario and calls PlayScenario().
        /// </summary>
        public void PlayRandomScenario()
        {
            if (_scenarioNames == null || _scenarioNames.Count == 0)
            {
                throw new InvalidOperationException(
                    "Unable to select random scenario. " +
                    "There is no list to choose from or it is empty.");
            }

            int index = RNG.Next(0, _scenarioNames.Count);
            SetSelectedIndex(index);
            PlayScenario();
        }

        /// <summary>
        /// Sets the currently selected scenario.
        /// </summary>
        /// <param name="index"></param>
        private void SetSelectedIndex(int index)
        {
            if (index < 0 || _scenarioNames == null || index >= _scenarioNames.Count)
            {
                throw new ArgumentOutOfRangeException(
                    "index", "Tried to set selection to invalid index.");
            }

            _selectedIndex = index;

            if (_playButton) _playButton.interactable = true;
        }

        /// <summary>
        /// Fills the lists of scenario names and paths.
        /// Creates a list item (UI) for each scenario.
        /// </summary>
        /// <returns>List is not empty</returns>
        private bool PopulateList()
        {
            ScenarioList.GetScenarioNames(out _scenarioNames, out _scenarioPaths);
            if (_scenarioNames.Count == 0) return false;

            float y = 0;
            for (var i = 0; i < _scenarioNames.Count; i++)
            {
                CreateListItem(i, y);
                y -= _listItemMargin + _listItemHeight;
            }

            // Set height of content panel to fit its children
            SetContentHeight(Mathf.Abs(y));

            return true; // List was not empty
        }

        /// <summary>
        /// Creates a list item of the specified scenario index.
        /// </summary>
        /// <param name="index">Index of the scenario</param>
        /// <param name="y">Y-coordinate of the list item in the content window</param>
        private void CreateListItem(int index, float y)
        {
            GameObject listItem = Instantiate(_listItemPrefab, _content.transform);

            // Set text of ListItem
            var textComponent = listItem.GetComponentInChildren<TextMeshProUGUI>();
            if (!textComponent)
            {
                throw new NullReferenceException("Unable to find text component.");
            }
            textComponent.text = _scenarioNames[index];

            // Set button click to update the current selected index
            var button = listItem.GetComponent<Button>();
            if (button == null)
            {
                throw new InvalidCastException(
                    "Cannot get Button from ListItem. It needs one to be selectable.");
            }
            button.onClick.AddListener(() => SetSelectedIndex(index));

            // Set position of ListItem
            var rect = listItem.transform as RectTransform;
            if (rect == null)
            {
                throw new InvalidCastException(
                    "Cannot get RectTransform. Is this prefab an UI element?");
            }
            rect.localPosition = new Vector3(
                rect.position.x + rect.pivot.x * rect.rect.width,
                y - (1f - rect.pivot.y) * rect.rect.height,
                rect.localPosition.z);
        }

        /// <summary>
        /// Set the height of the content container to fit all the list items.
        /// </summary>
        /// <param name="height"></param>
        private void SetContentHeight(float height)
        {
            var contentRect = _content.transform as RectTransform;
            if (contentRect == null)
            {
                throw new InvalidCastException(
                    "Cannot get RectTransform. Is the Content GameObject an UI element?");
            }

            Vector2 oldSize = contentRect.rect.size;
            Vector2 deltaSize = new Vector2(oldSize.x, height - _listItemMargin) - oldSize;
            contentRect.offsetMin = contentRect.offsetMin -
                                    new Vector2(deltaSize.x * contentRect.pivot.x,
                                        deltaSize.y * contentRect.pivot.y);
            contentRect.offsetMax = contentRect.offsetMax +
                                    new Vector2(deltaSize.x * (1f - contentRect.pivot.x),
                                        deltaSize.y * (1f - contentRect.pivot.y));
        }
    }
}