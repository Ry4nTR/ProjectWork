using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITSProjectWork
{
    [Serializable]
    public class ListCheckManager<ElementType>
    {
        [Serializable]
        public struct ElementCheck
        {
            public ElementType element;
            public bool isCompleted;
        }

        public static event Action OnListCompleted = delegate { };

        //TODO: Add a list of elements to be checked instead of using a number (letto action could be read accidentally)
        [Tooltip("The amount of elements that need to be approved before the list is considered complete. Set to 0 to disable this check.")]
        [SerializeField] private int itemAmountToApprove;
        [SerializeField] protected List<ElementCheck> _items;

        public IReadOnlyList<ElementCheck> Items => _items;

        public bool IsListCompleted()
        {
            int completedCount = 0;
            foreach (var item in _items)
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

        public void SetItemCompleted(ElementType item)
        {
            int itemIndex = _items.FindIndex(x => x.element.Equals(item));
            if(itemIndex == -1)
            {
                Debug.LogError($"Item {item} not found in the list.");
                return;
            }
            ElementCheck itemSelected = _items[itemIndex];
            itemSelected.isCompleted = true;
            _items[itemIndex] = itemSelected;

            if (IsListCompleted())
            {
                //TODO: Unlock the next part of the game
                OnListCompleted?.Invoke();
            }
        }
    }
}