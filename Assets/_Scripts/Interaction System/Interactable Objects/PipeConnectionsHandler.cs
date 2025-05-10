using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    /// <summary>
    /// This class manages all the trigger connections inside a single pipe (in case of a curved pipe with 2 connections for example).
    /// </summary>
    public class PipeConnectionsHandler : MonoBehaviour
    {
        public event Action<bool> OnPipeConnectionChanged;
        [Header("Visual")]
        [SerializeField] private Color pipeConnectedColor;
        [SerializeField] private Color pipeNotConnectedColor;
        [Header("Logic")]
        [SerializeField] private List<PipeConnectionTrigger> pipeConnectionTriggers;
        [SerializeField] private bool _isPipeFullyConnected;

        private MeshRenderer pipeRenderer;

        public bool IsPipeFullyConnected
        {
            get => _isPipeFullyConnected;
            private set
            {
                _isPipeFullyConnected = value;
                Debug.Log("Setting color...");
                pipeRenderer.material.SetColor("_BaseColor", _isPipeFullyConnected ? pipeConnectedColor : pipeNotConnectedColor);
                OnPipeConnectionChanged?.Invoke(_isPipeFullyConnected);
            }
        }

        private void Awake()
        {
            pipeRenderer = GetComponentInChildren<MeshRenderer>();
            SubscribeToAllTriggersEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromAllTriggersEvents();
        }

        public void SubscribeToAllTriggersEvents()
        {
            foreach (var pipeConnectionTrigger in pipeConnectionTriggers)
            {
                pipeConnectionTrigger.OnTriggerStatusChanged += CheckPipeStatus;
            }
        }
        public void UnsubscribeFromAllTriggersEvents()
        {
            foreach (var pipeConnectionTrigger in pipeConnectionTriggers)
            {
                pipeConnectionTrigger.OnTriggerStatusChanged -= CheckPipeStatus;
            }
        }
        private void CheckPipeStatus(bool isConnected)
        {
            if (!isConnected)
            {
                IsPipeFullyConnected = false;
                return;
            }

            // Check if all pipe connection triggers are connected
            bool allConnected = true;
            foreach (var pipeConnectionTrigger in pipeConnectionTriggers)
            {
                if (!pipeConnectionTrigger.IsConnected)
                {
                    allConnected = false;
                    break;
                }
            }
            IsPipeFullyConnected = allConnected;
        }
    }

}
