using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectWork
{
    public class  FirePuzzle : Puzzle
    {
        [SerializeField] private CheckListManager<Fire> firesCheckListManager;

        private void Awake()
        {
            List<Fire> fires = GetComponentsInChildren<Fire>(true).ToList();
            firesCheckListManager.InitializeCheckList(fires, true);

            SubscribeToFireEvents();
            firesCheckListManager.OnListCompleted += InvokePuzzleCompletedEvent;
        }

        private void OnDestroy()
        {
            UnsubscribeFromFireEvents();
            firesCheckListManager.OnListCompleted -= InvokePuzzleCompletedEvent;
        }

        private void SubscribeToFireEvents()
        {
            foreach(var fireTask in firesCheckListManager.Items)
            {
                fireTask.element.OnFireDead += CheckPuzzleStatus;
            }
        }

        private void UnsubscribeFromFireEvents()
        {
            foreach (var fireTask in firesCheckListManager.Items)
            {
                fireTask.element.OnFireDead -= CheckPuzzleStatus;
            }
        }

        private void CheckPuzzleStatus(Fire deadFire)
        {
            firesCheckListManager.TrySetItemCompleted(deadFire);
        }
    }

}
