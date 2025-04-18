using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class GameInteractionManager : MonoBehaviour
    {
        public static GameInteractionManager Instance { get; private set; }

        public static event Action<bool> OnTasksCompleted = delegate { };

        [SerializeField] private CheckListManager<InteractableObject> listCheckManager;
        [SerializeField] private int maxDays = 3;
        private int currentDay = 1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SubscribeToAllInteractionEnds();

                TrashManager.OnTrashSpawned += AddToListAndSubscribeToTrashThrownEvent;
                Bed.OnBedInteracted += ResetInteractions;
                listCheckManager.OnListCompleted += InvokeTasksCompletedEvent;
            }
            else
            {
                Destroy(gameObject);
            }   
        }

        private void Start()
        {
            currentDay = 1;
        }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            UnsubscribeToAllInteractionEnds();

            TrashManager.OnTrashSpawned -= AddToListAndSubscribeToTrashThrownEvent;
            listCheckManager.OnListCompleted -= InvokeTasksCompletedEvent;
        }

        private void InvokeTasksCompletedEvent()
        {
            currentDay++;
            OnTasksCompleted?.Invoke(currentDay > maxDays);
            if(currentDay > maxDays)
            {
                Debug.Log("Tutorial finished.");
            }
            else
            {
                Debug.Log($"Day {currentDay} completed. Unlocking bed interaction.");
            }    
        }

        private void AddToListAndSubscribeToTrashThrownEvent(Trash spawnedTrash)
        {
            listCheckManager.AddItemToCheckList(spawnedTrash);
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
            bedInteraction.ResetInteraction();
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