using System;
using UnityEngine;

namespace ProjectWork
{
    public abstract class Puzzle : MonoBehaviour
    {
        /// <summary>
        /// This event is used by the PuzzleChecker to update the checklist when a specific puzzle is completed.
        /// </summary>
        public static event Action<Puzzle> OnSpecificPuzzleCompleted;

        /// <summary>
        /// This event is used by the PipeRotator to lock the interaction when the puzzle is completed.
        /// </summary>
        public static event Action OnPuzzleCompleted;

        protected void InvokeCompletedEvent()
        {
            OnSpecificPuzzleCompleted?.Invoke(this);
            OnPuzzleCompleted?.Invoke();
        }
    }
}
