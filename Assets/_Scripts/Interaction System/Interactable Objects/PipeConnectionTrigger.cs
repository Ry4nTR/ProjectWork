using System;
using UnityEngine;

namespace ProjectWork
{
    /// <summary>
    /// This represents a single pipe connection trigger.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PipeConnectionTrigger : MonoBehaviour
    {
        [Header("Enigma stats")]
        [SerializeField] private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                _isConnected = value;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(transform.IsChildOf(other.transform))
            {
                return;
            }
            if (other.gameObject.CompareTag("Pipe_Normal"))
            {
                IsConnected = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (transform.IsChildOf(other.transform))
            {
                return;
            }
            if (other.gameObject.CompareTag("Pipe_Normal"))
            {
                IsConnected = false;
            }
        }
    }
}
