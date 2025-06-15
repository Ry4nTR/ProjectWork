using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProjectWork.UI
{
    public class MissionPanel : UI_Panel
    {     
        [Header("Objective Display")]
        [SerializeField] private string defaultObjectiveText = "Reach the Central Room";
        [SerializeField] private TMP_Text _objectivesText;
        [SerializeField] private float _updateInterval = 0.2f;

        private float _updateTimer;

        protected override void Awake()
        {
            base.Awake();
            if (_objectivesText != null)
            {
                _objectivesText.text = defaultObjectiveText;
            }            
        }

        private void OnEnable()
        {
            StartCoroutine(MonitorObjectives());
        }

        private IEnumerator Start()
        {
            while (ObjectiveManager.Instance == null)
            {
                Debug.LogWarning("MissionPanel: ObjectiveManager not found, waiting for it to initialize.");
                yield return null;
            }
            ObjectiveManager.Instance.OnObjectivesUpdated += UpdateMissions;
            ForceUpdateDisplay();
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnObjectivesUpdated -= UpdateMissions;
            }
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
                   (Time.time - _updateTimer) >= _updateInterval;
        }


        private void UpdateMissions(List<ObjectiveDisplayData> list)
        {           
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (ObjectiveManager.Instance.activeChecklists.Count == 0)
            {
                _objectivesText.text = defaultObjectiveText;
                return;
            }

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