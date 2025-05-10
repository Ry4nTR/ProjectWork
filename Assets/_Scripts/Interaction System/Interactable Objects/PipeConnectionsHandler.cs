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
        
        private PipeRotator pipeRotator;
        private bool _isPipeFullyConnected;

        private MeshRenderer pipeRenderer;

        public bool IsPipeFullyConnected
        {
            get => _isPipeFullyConnected;
            private set
            {
                _isPipeFullyConnected = value;
                pipeRenderer.material.SetColor("_BaseColor", _isPipeFullyConnected ? pipeConnectedColor : pipeNotConnectedColor);
                OnPipeConnectionChanged?.Invoke(_isPipeFullyConnected);
            }
        }

        private void Awake()
        {
            pipeRotator = GetComponentInChildren<PipeRotator>();
            pipeRenderer = GetComponentInChildren<MeshRenderer>();      

            pipeRotator.OnPipeEndRotation += CheckPipeStatus;
        }

        private void OnDestroy()
        {
            pipeRotator.OnPipeEndRotation -= CheckPipeStatus;
        }

        private void CheckPipeStatus()
        {
            Debug.Log($"{gameObject.name}: Calling Pipe connection status check");

            bool allConnected = true;
            foreach (var pipeConnectionTrigger in pipeConnectionTriggers)
            {
                Debug.Log($"{gameObject.name}: Pipe connection trigger status: {pipeConnectionTrigger.IsConnected}");
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
