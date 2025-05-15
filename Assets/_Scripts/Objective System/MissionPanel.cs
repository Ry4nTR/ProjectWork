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
        [SerializeField] private float _updateInterval = 0.2f;

        private float _updateTimer;

        protected override void Awake()
        {
            base.Awake();
            if (_objectivesText != null)
            {
                _objectivesText.text = "Arriva alla sala di comando";
            }
        }

        private IEnumerator Start()
        {
            while (ObjectiveManager.Instance == null)
            {
                yield return null;
            }
            ForceUpdateDisplay();
        }

        private void OnEnable()
        {
            StartCoroutine(MonitorObjectives());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator MonitorObjectives()
        {
            var lastUpdateTime = Time.time;

            while (true)
            {
                if (ShouldUpdate())
                {
                    UpdateDisplay();
                    lastUpdateTime = Time.time;
                }
                yield return new WaitForSeconds(_updateInterval);
            }
        }

        private bool ShouldUpdate()
        {
            return ObjectiveManager.Instance != null &&
                   ObjectiveManager.Instance.activeChecklists.Count > 0 &&
                   (Time.time - _updateTimer) >= _updateInterval;
        }

        private void UpdateDisplay()
        {
            var checklist = ObjectiveManager.Instance.activeChecklists[0];
            var formattedText = new System.Text.StringBuilder();

            foreach (var item in checklist.Items)
            {
                var objectiveDef = item.element.objectiveDefinition;
                if (objectiveDef == null || !objectiveDef.isMandatory) continue;

                // Skip if this objective should be hidden when completed
                if (objectiveDef.hideWhenCompleted && item.isCompleted) continue;

                if (item.isCompleted)
                {
                    formattedText.AppendLine($"<color=yellow>- {objectiveDef.displayText}</color>");
                }
                else
                {
                    formattedText.AppendLine($"- {objectiveDef.displayText}");
                }
            }

            _objectivesText.text = formattedText.Length > 0 ? formattedText.ToString() : "No objectives";
            _updateTimer = Time.time;
        }

        public void ForceUpdateDisplay()
        {
            _updateTimer = 0f;
            UpdateDisplay();
        }
    }
}