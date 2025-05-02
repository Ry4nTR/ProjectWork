using System;
using UnityEngine;

namespace ProjectWork
{
    /// <summary>
    /// This represents a single pipe connection trigger.
    /// </summary>
    public class PipeConnectionTrigger : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when the trigger is connected
        /// </summary>
        public event Action<bool> OnTriggerStatusChanged;

        [Header("Enigma stats")]
        [SerializeField] private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                _isConnected = value;
                OnTriggerStatusChanged?.Invoke(_isConnected);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pipe_Normal"))
            {
                IsConnected = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Pipe_Normal"))
            {
                IsConnected = false;
            }
        }
    }
}
