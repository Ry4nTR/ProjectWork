using System;
using System.Collections.Generic;
using ProjectWork;
using UnityEngine;


public struct ObjectiveDisplayData
{
    public string Text;
    public bool IsCompleted;
}


public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    public event Action<List<ObjectiveDisplayData>> OnObjectivesUpdated = delegate { };

    private List<CheckListManager<InteractableObject>> activeChecklists = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterChecklist(CheckListManager<InteractableObject> checklist)
    {
        if (!activeChecklists.Contains(checklist))
        {
            activeChecklists.Add(checklist);
            checklist.OnListCompleted += HandleChecklistCompletion;
            UpdateObjectives();
        }
    }

    public void UnregisterChecklist(CheckListManager<InteractableObject> checklist)
    {
        if (activeChecklists.Remove(checklist))
        {
            checklist.OnListCompleted -= HandleChecklistCompletion;
            UpdateObjectives();
        }
    }

    private void HandleChecklistCompletion() => UpdateObjectives();

    private void UpdateObjectives()
    {
        var displayData = new List<ObjectiveDisplayData>();

        foreach (var checklist in activeChecklists)
        {
            foreach (var item in checklist.Items)
            {
                if (item.element.objectiveDefinition != null && item.element.objectiveDefinition.isMandatory)
                {
                    displayData.Add(new ObjectiveDisplayData
                    {
                        Text = item.element.objectiveDefinition.displayText,
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