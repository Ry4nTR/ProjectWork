using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class PipePuzzleManager : MonoBehaviour
    {
        public static event Action OnPuzzleCompleted;

        [SerializeField] private List<PipeConnectionsHandler> pipeConnectionsHandlers;

        private void Awake()
        {
            SubscribeToAllPipesEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromAllPipesEvents();
        }

        private void SubscribeToAllPipesEvents()
        {
            foreach(var pipeConnectionTrigger in pipeConnectionsHandlers)
            {
                pipeConnectionTrigger.OnPipeConnectionChanged += CheckPuzzleStatus;
            }
        }

        private void UnsubscribeFromAllPipesEvents()
        {
            foreach (var pipeConnectionTrigger in pipeConnectionsHandlers)
            {
                pipeConnectionTrigger.OnPipeConnectionChanged -= CheckPuzzleStatus;
            }
        }

        private void CheckPuzzleStatus(bool isConnected)
        {
            if (!isConnected)
            {
                return;
            }
            // Check if all pipe connection triggers are connected
            bool allPipesConnected = true;
            foreach (var pipeConnectionsHandler in pipeConnectionsHandlers)
            {
                if (!pipeConnectionsHandler.IsPipeFullyConnected)
                {
                    allPipesConnected = false;
                    break;
                }
            }
            if (allPipesConnected)
            {
                Debug.Log("All pipes are connected!");
                // Trigger the puzzle completion event or logic here
                OnPuzzleCompleted?.Invoke();
            }
        }
    }

}
