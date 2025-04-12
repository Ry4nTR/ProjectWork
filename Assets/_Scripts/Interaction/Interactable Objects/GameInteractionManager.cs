using System.Collections.Generic;
using UnityEngine;

namespace ITSProjectWork
{
    public class GameInteractionManager : MonoBehaviour
    {
        public static GameInteractionManager Instance { get; private set; }

        [SerializeField] private ListCheckManager<InteractableObject> listCheckManager;
        [SerializeField] private Bed bedInteraction;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SubscribeToAllInteractionEnds();
                bedInteraction.OnInteractionFinished += ResetInteractions;
                listCheckManager.OnListCompleted += UnlockBedInteraction;
            }
            else
            {
                Destroy(gameObject);
            }   
        }

        private void OnDestroy()
        {
            UnsubscribeToAllInteractionEnds();
            listCheckManager.OnListCompleted -= UnlockBedInteraction;
        }

        private void ResetInteractions(InteractableObject eventInvoker)
        {
            
            foreach (ListCheckManager<InteractableObject>.ElementCheck item in listCheckManager.Items)
            {
                item.element.ResetInteraction();
            }
            listCheckManager.ResetItemCompletedList();
            bedInteraction.ResetInteraction();
        }

        private void UnlockBedInteraction()
        {
            bedInteraction.UnlockInteraction();
        }

        private void UnsubscribeToAllInteractionEnds()
        {
            foreach (ListCheckManager<InteractableObject>.ElementCheck item in listCheckManager.Items)
            {
                item.element.OnInteractionFinished -= SetItemCompletedInList;
            }
        }

        private void SubscribeToAllInteractionEnds()
        {
            foreach (ListCheckManager<InteractableObject>.ElementCheck item in listCheckManager.Items)
            {
                item.element.OnInteractionFinished += SetItemCompletedInList;
            }
        }

        /// <summary>
        /// Using this method to avoid losing track of lambdas
        /// </summary>
        private void SetItemCompletedInList(InteractableObject eventInvoker)
        {
            listCheckManager.SetItemCompleted(eventInvoker);
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