using System;
using UnityEngine;

namespace ProjectWork
{
    public abstract class Puzzle : MonoBehaviour
    {
        public static event Action OnPuzzleCompleted;

        protected void InvokeCompletedEvent()
        {
            OnPuzzleCompleted?.Invoke();
        }
    }
}
