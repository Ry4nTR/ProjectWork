using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class GameInteractionManager : MonoBehaviour
    {
        public static GameInteractionManager Instance { get; private set; }

        [SerializeField] private CheckListManager<InteractableObject> listCheckManager;
        [SerializeField] private Bed bedInteraction;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SubscribeToAllInteractionEnds();
                bedInteraction.OnInteractionFinished += ResetInteractions;
                listCheckManager.OnListCompleted += bedInteraction.UnlockInteraction;
            }
            else
            {
                Destroy(gameObject);
            }   
        }

        private void OnDestroy()
        {
            UnsubscribeToAllInteractionEnds();
            listCheckManager.OnListCompleted -= bedInteraction.UnlockInteraction;
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
            foreach (ListCheckManager<InteractableObject>.ElementCheck item in listCheckManager.Items)
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