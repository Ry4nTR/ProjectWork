using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class ObjectiveManager : MonoBehaviour
    {
        public static ObjectiveManager Instance { get; private set; }

        // Serializable struct to define how different types should be displayed
        [Serializable]
        public struct TypeDisplayConfig
        {
            public Type type;
            public string displayFormat; // e.g., "Collect {0}" where {0} will be the item name
        }

        [SerializeField] private List<TypeDisplayConfig> _displayConfigs = new List<TypeDisplayConfig>();

        // Current checklist managers being tracked
        private List<object> _activeChecklistManagers = new List<object>();

        // Event to notify HUD when objectives change
        public event Action<List<ObjectiveDisplayData>> OnObjectivesUpdated = delegate { };

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

        /// <summary>
        /// Register a new checklist manager to be tracked
        /// </summary>
        public void RegisterChecklistManager<T>(CheckListManager<T> manager, string displayFormat = null)
        {
            if (!_activeChecklistManagers.Contains(manager))
            {
                _activeChecklistManagers.Add(manager);

                // Add default display config if none exists for this type
                if (!HasDisplayConfig(typeof(T)) && !string.IsNullOrEmpty(displayFormat))
                {
                    _displayConfigs.Add(new TypeDisplayConfig
                    {
                        type = typeof(T),
                        displayFormat = displayFormat
                    });
                }

                // Subscribe to events
                manager.OnListCompleted += HandleListCompleted;
                UpdateObjectiveDisplay();
            }
        }

        /// <summary>
        /// Unregister a checklist manager
        /// </summary>
        public void UnregisterChecklistManager<T>(CheckListManager<T> manager)
        {
            if (_activeChecklistManagers.Contains(manager))
            {
                _activeChecklistManagers.Remove(manager);
                manager.OnListCompleted -= HandleListCompleted;
                UpdateObjectiveDisplay();
            }
        }

        private bool HasDisplayConfig(Type type)
        {
            foreach (var config in _displayConfigs)
            {
                if (config.type == type)
                {
                    return true;
                }
            }
            return false;
        }

        private string GetDisplayFormat(Type type)
        {
            foreach (var config in _displayConfigs)
            {
                if (config.type == type)
                {
                    return config.displayFormat;
                }
            }
            return "{0}"; // Default format if none specified
        }

        private void HandleListCompleted()
        {
            UpdateObjectiveDisplay();
        }

        /// <summary>
        /// Generate display data for all active checklists
        /// </summary>
        private void UpdateObjectiveDisplay()
        {
            var displayData = new List<ObjectiveDisplayData>();

            foreach (var manager in _activeChecklistManagers)
            {
                // Use reflection to access the Items property generically
                var itemsProperty = manager.GetType().GetProperty("Items");
                if (itemsProperty != null)
                {
                    var items = itemsProperty.GetValue(manager) as System.Collections.IEnumerable;
                    var itemType = GetItemType(manager.GetType());

                    if (items != null && itemType != null)
                    {
                        string displayFormat = GetDisplayFormat(itemType);

                        foreach (var item in items)
                        {
                            // Use reflection to access the element and isCompleted fields
                            var elementField = item.GetType().GetField("element");
                            var isCompletedField = item.GetType().GetField("isCompleted");

                            if (elementField != null && isCompletedField != null)
                            {
                                var element = elementField.GetValue(item);
                                bool isCompleted = (bool)isCompletedField.GetValue(item);

                                string displayText = string.Format(displayFormat, element.ToString());
                                displayData.Add(new ObjectiveDisplayData
                                {
                                    Text = displayText,
                                    IsCompleted = isCompleted
                                });
                            }
                        }
                    }
                }
            }

            OnObjectivesUpdated?.Invoke(displayData);
        }

        private Type GetItemType(Type checklistManagerType)
        {
            // Get the generic type argument from CheckListManager<T>
            if (checklistManagerType.IsGenericType &&
                checklistManagerType.GetGenericTypeDefinition() == typeof(CheckListManager<>))
            {
                return checklistManagerType.GetGenericArguments()[0];
            }
            return null;
        }
    }

    /// <summary>
    /// Data structure for HUD display
    /// </summary>
    public struct ObjectiveDisplayData
    {
        public string Text;
        public bool IsCompleted;
    }
}