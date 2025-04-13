using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class GameInteractionManager : MonoBehaviour
    {
        public static GameInteractionManager Instance { get; private set; }

        public static event Action OnTutorialFinished = delegate { };

        [SerializeField] private CheckListManager<InteractableObject> listCheckManager;
        [SerializeField] private Bed bedInteraction;
        [SerializeField] private int maxDays = 3;
        private int currentDay = 1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SubscribeToAllInteractionEnds();

                TrashManager.OnTrashSpawned += AddToListAndSubscribeToTrashThrownEvent;
                bedInteraction.OnInteractionFinished += ResetInteractions;
                listCheckManager.OnListCompleted += UnlockBedInteraction;
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
            bedInteraction.OnInteractionFinished -= ResetInteractions;
            listCheckManager.OnListCompleted -= UnlockBedInteraction;
        }

        private void UnlockBedInteraction()
        {
            currentDay++;
            if(currentDay > maxDays)
            {
                OnTutorialFinished?.Invoke();
            }
            else
            {
                bedInteraction.UnlockInteraction();
            }    
        }

        private void AddToListAndSubscribeToTrashThrownEvent(Trash spawnedTrash)
        {
            listCheckManager.AddItemToCheckList(spawnedTrash);
        }
        
        private void ResetInteractions(InteractableObject eventInvoker)
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