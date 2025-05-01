using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectWork
{
    public class TutorialTaskChecker : MonoBehaviour
    {
        public static TutorialTaskChecker Instance { get; private set; }
        
        public static event Action OnTasksCompleted = delegate { };
        public static event Action<bool> OnDayPassed = delegate { };    //True = days finished, false = days not finished

        [SerializeField] private int maxDays = 3;
        [Tooltip("This list contains all the tasks that are always present in the tutorial phase for each day. They will be added to the checklist at the start of the game.")]
        [SerializeField] private List<InteractableObject> permanentTasks;
        [SerializeField] private CheckListManager<InteractableObject> currentChecklist;
        
        private int _currentDay = 1;
        
        public int CurrentDay 
        { 
            get => _currentDay;
            private set
            {
                _currentDay = value;
                if(_currentDay > maxDays)
                {
                    LockInteractions();
                }
                else
                {
                    ResetInteractions();
                }    
                OnDayPassed?.Invoke(_currentDay > maxDays);
            }
        }

        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                foreach (var task in permanentTasks)
                {
                    currentChecklist.AddItemToCheckList(task, true, false);
                }
                SubscribeToAllInteractionEnds();

                FoodPad.OnSelectedFood += SetOrderTaskCompleted;
                Prisoner.OnDialogueFinished += SetDialogueTaskCompleted;
                Trash.OnTrashThrown += SetTrashInteractionCompleted;
                Bed.OnBedInteracted += IncreaseDay;
                currentChecklist.OnListCompleted += InvokeTasksCompletedEvent;
            }
            else
            {
                Destroy(gameObject);
            }   
        }

        private void OnEnable()
        {
            ObjectiveManager.Instance.RegisterChecklist(currentChecklist);
        }

        private void Start()
        {
            _currentDay = 1;
            
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                ObjectiveManager.Instance.UnregisterChecklist(currentChecklist);
                return;
            }

            UnsubscribeToAllInteractionEnds();

            FoodPad.OnSelectedFood -= SetOrderTaskCompleted;
            Prisoner.OnDialogueFinished -= SetDialogueTaskCompleted;
            Trash.OnTrashThrown -= SetTrashInteractionCompleted;
            Bed.OnBedInteracted -= IncreaseDay;
            currentChecklist.OnListCompleted -= InvokeTasksCompletedEvent;
        }

        private void IncreaseDay()
        {
            CurrentDay++;
            ResetObjectivesForNewDay(); // Add this line
        }

        private void ResetObjectivesForNewDay()
        {
            // Reset all objectives in the checklist
            currentChecklist.ResetItemCompletedList();

            // Reset interactions for all objects
            foreach (var item in currentChecklist.Items)
            {
                item.element.ResetInteraction();
            }

            // Force the HUD to update
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.ForceUpdateObjectives();
            }

            Debug.Log("Objectives reset for new day");
        }

        private void InvokeTasksCompletedEvent() => OnTasksCompleted?.Invoke();
        
        private void SetTrashInteractionCompleted(Trash spawnedTrash)
        {
            currentChecklist.SetItemCompleted(spawnedTrash);
            // Forza l'aggiornamento dell'HUD
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.ForceUpdateObjectives();
            }
        }

        private void SetDialogueTaskCompleted(Prisoner prisoner) => currentChecklist.SetItemCompleted(prisoner);
        
        private void SetOrderTaskCompleted(FoodType _)
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in currentChecklist.Items)
            {
                if (item.element is FoodPad foodPad)
                {
                    currentChecklist.SetItemCompleted(foodPad);
                    break;
                }
            }
        }

        private void ResetInteractions()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in currentChecklist.Items)
            {
                item.element.ResetInteraction();
            }
            currentChecklist.ResetItemCompletedList();
        }

        private void LockInteractions()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in currentChecklist.Items)
            {
                item.element.LockInteraction();
            }
        }

        private void UnsubscribeToAllInteractionEnds()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in currentChecklist.Items)
            {
                item.element.OnInteractionFinished -= currentChecklist.SetItemCompleted;
            }
        }

        private void SubscribeToAllInteractionEnds()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in currentChecklist.Items)
            {
                item.element.OnInteractionFinished += currentChecklist.SetItemCompleted;
            }
        }

        public bool IsItemCompleted(InteractableObject item)
        {
            foreach(var listItem in currentChecklist.Items)
            {
                if (listItem.element == item)
                {
                    return listItem.isCompleted;
                }
            }
            return false;
        }
    }
}