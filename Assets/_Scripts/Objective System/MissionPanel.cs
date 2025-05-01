using UnityEngine;
using TMPro;
using System.Collections.Generic;
using ProjectWork;

namespace ProjectWork.UI
{
    public class MissionPanel : UI_Panel
    {
        [Header("Objective Display")]
        [SerializeField] private TMP_Text _objectivesText;
        [SerializeField] private float _updateInterval = 0.2f; // Prevents too frequent updates

        private float _updateTimer;
        private string _currentObjectives = "";

        private void Start()
        {
            // Forza un aggiornamento all'avvio per catturare gli obiettivi iniziali
            ForceUpdateDisplay();
        }

        protected override void Awake()
        {
            base.Awake();

            // Initialize with current objectives if available
            if (_objectivesText != null)
            {
                UpdateObjectivesText(ObjectiveManager.Instance.GetCurrentObjectives());
            }
            else if (_objectivesText != null)
            {
                _objectivesText.text = "No current objectives";
            }
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