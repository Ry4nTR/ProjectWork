using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public struct ObjectiveDisplayData
    {
        public string Text;
        public bool IsCompleted;
    }

    public class ObjectiveManager : MonoBehaviour
    {
        public static ObjectiveManager Instance { get; private set; }

        public event Action<List<ObjectiveDisplayData>> OnObjectivesUpdated = delegate { };

        public List<CheckListManager<InteractableObject>> activeChecklists = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void RegisterChecklist(CheckListManager<InteractableObject> checklist)
        {
            if (!activeChecklists.Contains(checklist))
            {
                activeChecklists.Add(checklist);
                checklist.OnListCompleted += UpdateObjectiveDisplay;
                UpdateObjectiveDisplay();
            }
        }

        public void UnregisterChecklist(CheckListManager<InteractableObject> checklist)
        {
            if (activeChecklists.Remove(checklist))
            {
                checklist.OnListCompleted -= UpdateObjectiveDisplay;
                UpdateObjectiveDisplay();
            }
        }

        private void UpdateObjectiveDisplay()
        {
            var displayData = new List<ObjectiveDisplayData>();

            foreach (var checklist in activeChecklists)
            {
                foreach (var item in checklist.Items)
                {
                    var objectiveDef = item.element.objectiveDefinition;
                    if (objectiveDef == null || !objectiveDef.isMandatory) continue;

                    if (!(objectiveDef.hideWhenCompleted && item.isCompleted))
                    {
                        displayData.Add(new ObjectiveDisplayData
                        {
                            Text = objectiveDef.displayText,
                            IsCompleted = item.isCompleted
                        });
                    }
                }
            }

            OnObjectivesUpdated?.Invoke(displayData);
        }

        public List<ObjectiveDisplayData> GetCurrentObjectives()
        {
            var currentObjectives = new List<ObjectiveDisplayData>();

            foreach (var checklist in activeChecklists)
            {
                foreach (var item in checklist.Items)
                {
                    if (item.element.objectiveDefinition != null && item.element.objectiveDefinition.isMandatory)
                    {
                        currentObjectives.Add(new ObjectiveDisplayData
                        {
                            Text = item.element.objectiveDefinition.displayText,
                            IsCompleted = item.isCompleted
                        });
                    }
                }
            }

            return currentObjectives;
        }

        public void ForceUpdateObjectives()
        {
            var currentObjectives = GetCurrentObjectives();
            OnObjectivesUpdated?.Invoke(currentObjectives);
        }
    }
}