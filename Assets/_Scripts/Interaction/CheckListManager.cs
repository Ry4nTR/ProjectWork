using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    [Serializable]
    /// <summary>
    /// This class is used to manage a list of elements that need to be checked.
    /// </
    public class CheckListManager<ItemType>
    {
        [Serializable]
        /// <summary>
        /// Contains the generic element and a boolean to check if it is completed or not.
        /// </summary>
        public struct ItemCheck
        {
            public ItemType element;
            public bool isCompleted;
        }

        public event Action OnListCompleted = delegate { };

        [Tooltip("The amount of elements that need to be approved before the list is considered complete. Set to 0 to disable this check.")]
        [SerializeField] private int itemAmountToApprove;
        [SerializeField] protected List<ItemCheck> _allItems;
        public IReadOnlyList<ItemCheck> Items => _allItems;

        /// <summary>
        /// Checks if the list is completed based on the amount of completed items that is equal or greater than the amount of items to approve.
        /// </summary>
        /// <returns>True if the minimum number of Completed task is reached</returns>
        public bool AreMinimumNumberOfItemsCompleted()
        {
            int completedCount = 0;
            foreach (var item in _allItems)
            {
                if (item.isCompleted)
                {
                    completedCount++;
                    if (completedCount >= itemAmountToApprove)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the list is fully completed. This is used to check if the player has completed all the items in the list.
        /// </summary>
        /// <returns>True if all items are marked as Completed</returns>
        public bool IsListFullyCompleted()
        {
            foreach (var item in _allItems)
            {
                if (!item.isCompleted)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Sets the item as completed. Will be used when the player interacts with the item or completes an enigma.
        /// </summary>
        public void SetItemCompleted(ItemType item)
        {
            int itemIndex = _allItems.FindIndex(x => x.element.Equals(item));
            if(itemIndex == -1)
            {
                Debug.LogError($"Item {item} not found in the list.");
                return;
            }
            ItemCheck itemSelected = _allItems[itemIndex];
            itemSelected.isCompleted = true;
            _allItems[itemIndex] = itemSelected;

            if (AreMinimumNumberOfItemsCompleted())
            {
                //TODO: Unlock the next part of the game
                OnListCompleted?.Invoke();
            }
        }

        /// <summary>
        /// Resets the list of items to be checked. This is used when the player needs to redo the list.
        /// </summary>
        public void ResetItemCompletedList()
        {
            for (int i = 0; i < _allItems.Count; i++)
            {
                ItemCheck item = _allItems[i];
                item.isCompleted = false;
                _allItems[i] = item;
            }
        }

