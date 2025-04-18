using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class GameInteractionManager : MonoBehaviour
    {
        public static GameInteractionManager Instance { get; private set; }
        
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

                TrashManager.OnTrashSpawned += AddToListAndSubscribeToTrashThrownEvent;
                Bed.OnBedInteracted += IncreaseDay;
                listCheckManager.OnListCompleted += InvokeTasksCompletedEvent;
            }
            else
            {
                Destroy(gameObject);
            }   
        }

        private void Start()
        {
            _currentDay = 1;
        }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            UnsubscribeToAllInteractionEnds();

            TrashManager.OnTrashSpawned -= AddToListAndSubscribeToTrashThrownEvent;
            Bed.OnBedInteracted -= IncreaseDay;
            listCheckManager.OnListCompleted -= InvokeTasksCompletedEvent;
        }

        private void IncreaseDay() => CurrentDay++;

        private void InvokeTasksCompletedEvent() => OnTasksCompleted?.Invoke();

        private void AddToListAndSubscribeToTrashThrownEvent(Trash spawnedTrash)
        {
            listCheckManager.AddItemToCheckList(spawnedTrash, false);
            Trash.OnTrashThrown += SetTrashInteractionCompleted;
        }

        private void SetTrashInteractionCompleted(Trash spawnedTrash)
        {
            listCheckManager.SetItemCompleted(spawnedTrash);
            Trash.OnTrashThrown -= SetTrashInteractionCompleted;
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