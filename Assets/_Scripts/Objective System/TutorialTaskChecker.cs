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

        [SerializeField] private CheckListManager<InteractableObject> listCheckManager;
        [SerializeField] private int maxDays = 3;
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
                
                SubscribeToAllInteractionEnds();

                FoodPad.OnSelectedFood += SetOrderTaskCompleted;
                Prisoner.OnDialogueFinished += SetDialogueTaskCompleted;
                Trash.OnTrashThrown += SetTrashInteractionCompleted;
                Bed.OnBedInteracted += IncreaseDay;
                listCheckManager.OnListCompleted += InvokeTasksCompletedEvent;
            }
            else
            {
                Destroy(gameObject);
            }   
        }

        private void OnEnable()
        {
            ObjectiveManager.Instance.RegisterChecklist(listCheckManager);
        }

        private void Start()
        {
            _currentDay = 1;

            // Forza l'aggiornamento degli obiettivi all'avvio
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.ForceUpdateObjectives();
            }
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                ObjectiveManager.Instance.UnregisterChecklist(listCheckManager);
                return;
            }

            UnsubscribeToAllInteractionEnds();

            FoodPad.OnSelectedFood -= SetOrderTaskCompleted;
            Prisoner.OnDialogueFinished -= SetDialogueTaskCompleted;
            Trash.OnTrashThrown -= SetTrashInteractionCompleted;
            Bed.OnBedInteracted -= IncreaseDay;
            listCheckManager.OnListCompleted -= InvokeTasksCompletedEvent;
        }

        private void IncreaseDay()
        {
            CurrentDay++;
            ResetObjectivesForNewDay(); // Add this line
        }

        private void ResetObjectivesForNewDay()
        {
            // Reset all objectives in the checklist
            listCheckManager.ResetItemCompletedList();

            // Reset interactions for all objects
            foreach (var item in listCheckManager.Items)
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
            listCheckManager.SetItemCompleted(spawnedTrash);
            // Forza l'aggiornamento dell'HUD
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.ForceUpdateObjectives();
            }
        }

        private void SetDialogueTaskCompleted(Prisoner prisoner) => listCheckManager.SetItemCompleted(prisoner);
        
        private void SetOrderTaskCompleted(FoodType _)
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in listCheckManager.Items)
            {
                if (item.element is FoodPad foodPad)
                {
                    listCheckManager.SetItemCompleted(foodPad);
                    break;
                }
            }
        }

        private void ResetInteractions()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in listCheckManager.Items)
            {
                item.element.ResetInteraction();
            }
            listCheckManager.ResetItemCompletedList();
        }

        private void LockInteractions()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in listCheckManager.Items)
            {
                item.element.LockInteraction();
            }
        }

        private void UnsubscribeToAllInteractionEnds()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in listCheckManager.Items)
            {
                item.element.OnInteractionFinished -= listCheckManager.SetItemCompleted;
            }
        }

        private void SubscribeToAllInteractionEnds()
        {
            foreach (CheckListManager<InteractableObject>.ItemCheck item in listCheckManager.Items)
            {
                item.element.OnInteractionFinished += listCheckManager.SetItemCompleted;
            }
        }

        public bool IsItemCompleted(InteractableObject item)
        {
            foreach(var listItem in listCheckManager.Items)
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