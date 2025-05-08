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
            public bool isPermanent; // If true, the item will not be removed from the list when completed
        }

        public event Action OnListCompleted = delegate { };

        [SerializeField] protected List<ItemCheck> _allItems;
        public IReadOnlyList<ItemCheck> Items => _allItems;

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
        /// Try to set the item as completed if it is found.
        /// </summary>
        public bool TrySetItemCompleted(ItemType item)
        {
            int itemIndex = _allItems.FindIndex(x => x.element.Equals(item));
            if(itemIndex == -1)
            {
                Debug.LogError($"Item {item} not found in the list.");
                return false;
            }
            ItemCheck itemSelected = _allItems[itemIndex];
            itemSelected.isCompleted = true;
            _allItems[itemIndex] = itemSelected;

            if (IsListFullyCompleted())
            {
                //TODO: Unlock the next part of the game
                OnListCompleted?.Invoke();
            }
            return true;
        }

        /// <summary>
        /// Sets the item as completed. Will be used when the player interacts with the item or completes an enigma.
        /// </summary>
        public void SetItemCompleted(ItemType item)
        {
            int itemIndex = _allItems.FindIndex(x => x.element.Equals(item));
            if (itemIndex == -1)
            {
                Debug.LogError($"Item {item} not found in the list.");
                return;
            }
            ItemCheck itemSelected = _allItems[itemIndex];
            itemSelected.isCompleted = true;
            _allItems[itemIndex] = itemSelected;

            // Optional: Still keep the full completion check
            if (IsListFullyCompleted())
            {
                Debug.Log("All objectives completed!");
                OnListCompleted?.Invoke();
            }
        }

        /// <summary>
        /// Resets the list of items. This is used to reset the list when the player starts a new day.
        /// </summary>
        public void ResetItemCompletedList()
        {
            _allItems.RemoveAll(x => !x.isPermanent); // Remove all items that are not permanent

            for (int i = 0; i < _allItems.Count; i++)
            {
                var item = _allItems[i];
                item.isCompleted = false;
                _allItems[i] = item;
            }
        }

        public bool TryAddItemToCheckList(ItemType item, bool isPermanent, bool canAddDuplicates)
        {
            ItemCheck newItem = new()
            {
                element = item,
                isCompleted = false,
                isPermanent = isPermanent
            };
            if (!canAddDuplicates && _allItems.Contains(newItem))
                return false;

            _allItems.Add(newItem);
            return true;
        }
    }
}