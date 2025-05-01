using ProjectWork;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveHUD : MonoBehaviour
{
    [SerializeField] private Transform _objectiveContainer;
    [SerializeField] private GameObject _objectivePrefab;

    private void Start()
    {
        ObjectiveManager.Instance.OnObjectivesUpdated += UpdateObjectivesDisplay;
    }

    private void OnDestroy()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnObjectivesUpdated -= UpdateObjectivesDisplay;
        }
    }

    private void UpdateObjectivesDisplay(List<ObjectiveDisplayData> objectives)
    {
        // Clear existing objectives
        foreach (Transform child in _objectiveContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new objective displays
        foreach (var objective in objectives)
        {
            var obj = Instantiate(_objectivePrefab, _objectiveContainer);
            var textComp = obj.GetComponentInChildren<TMP_Text>();
            textComp.text = objective.Text;
            textComp.fontStyle = objective.IsCompleted ? FontStyles.Strikethrough : FontStyles.Normal;
            textComp.color = objective.IsCompleted ? Color.gray : Color.white;
        }
    }
}