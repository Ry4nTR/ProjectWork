using UnityEngine;
using TMPro;
using System.Collections.Generic;
using ProjectWork;
using System.Collections;

namespace ProjectWork.UI
{
    public class MissionPanel : UI_Panel
    {
        [Header("Objective Display")]
        [SerializeField] private TMP_Text _objectivesText;
        [SerializeField] private float _updateInterval = 0.2f; // Prevents too frequent updates

        private float _updateTimer;
        private string _currentObjectives = "";

        protected override void Awake()
        {
            base.Awake();

            // Initialize with empty text
            if (_objectivesText != null)
            {
                _objectivesText.text = "Loading objectives...";
            }
        }

        private IEnumerator Start()
        {
            // Wait until ObjectiveManager is ready
            while (ObjectiveManager.Instance == null)
            {
                yield return null;
            }

            // Now force an update
            ForceUpdateDisplay();
        }

        private void OnEnable()
        {
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnObjectivesUpdated += UpdateObjectivesText;
            }

            // Add subscription to day change event
            TutorialTaskChecker.OnDayPassed += HandleNewDay;
        }

        private void OnDisable()
        {
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnObjectivesUpdated -= UpdateObjectivesText;
            }

            // Remove subscription
            TutorialTaskChecker.OnDayPassed -= HandleNewDay;
        }

        private void HandleNewDay(bool dayFinished)
        {
            // Force immediate update when day changes
            ForceUpdateDisplay();
        }

        private void Update()
        {
            // Throttle updates to prevent performance issues
            if (_updateTimer > 0)
            {
                _updateTimer -= Time.deltaTime;
            }
        }

        private void UpdateObjectivesText(List<ObjectiveDisplayData> objectives)
        {
            if (_objectivesText == null || _updateTimer > 0) return;

            _updateTimer = _updateInterval;
            _currentObjectives = FormatObjectives(objectives);
            _objectivesText.text = _currentObjectives;

            // Debug logging
            Debug.Log($"Updating objectives display. Count: {objectives?.Count}");
            if (objectives != null)
            {
                foreach (var obj in objectives)
                {
                    Debug.Log($"- {obj.Text} (Completed: {obj.IsCompleted})");
                }
            }
        }

        private string FormatObjectives(List<ObjectiveDisplayData> objectives)
        {
            if (objectives == null || objectives.Count == 0)
            {
                return "No objectives for today";
            }

            var formattedText = new System.Text.StringBuilder();
            foreach (var objective in objectives)
            {
                if (objective.IsCompleted)
                {
                    formattedText.AppendLine($"- <s>{objective.Text}</s>");
                }
                else
                {
                    formattedText.AppendLine($"- {objective.Text}");
                }
            }
            return formattedText.ToString();
        }

        // Optional: Call this to force immediate update
        public void ForceUpdateDisplay()
        {
            if (ObjectiveManager.Instance != null)
            {
                _updateTimer = 0;
                UpdateObjectivesText(ObjectiveManager.Instance.GetCurrentObjectives());
            }
        }
    }
}