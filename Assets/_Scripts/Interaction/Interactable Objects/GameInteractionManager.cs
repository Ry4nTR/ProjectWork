using UnityEngine;

namespace ITSProjectWork
{
    public class GameInteractionManager : MonoBehaviour
    {
        public static GameInteractionManager Instance { get; private set; }

        [SerializeField] private ListCheckManager<InteractableObject> listCheckManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SubscribeToAllInteractionEnds();
            }
            else
            {
                Destroy(gameObject);
            }   
        }

        private void OnDestroy()
        {
            UnsubscribeToAllInteractionEnds();
        }

        private void UnsubscribeToAllInteractionEnds()
        {
            foreach (var item in listCheckManager.Items)
            {
                item.element.OnInteractionFinished -= SetItemCompletedInList;
            }
        }

        private void SubscribeToAllInteractionEnds()
        {
            foreach (var item in listCheckManager.Items)
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