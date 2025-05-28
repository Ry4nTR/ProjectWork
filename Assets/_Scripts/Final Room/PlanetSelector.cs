using UnityEngine;

namespace ProjectWork
{
    public class PlanetSelector : InteractableObject
    {
        [Header("Planet Settings")]
        public bool isEarth = true;
        public WindowTrigger windowTrigger;

        [Header("Planet Info")]
        [SerializeField] private string planetName = "Planet";

        [Header("Decision Position")]
        [Tooltip("Transform that defines where this planet should move when selected for decision")]
        public Transform decisionTarget;

        protected override void Start()
        {
            base.Start();

            // Set the interaction prompt based on planet type
            SetInteractionPrompt();
        }

        private void SetInteractionPrompt()
        {
            string prompt = isEarth ? $"Select {planetName} (Earth)" : $"Select {planetName} (Mars)";
            SetInteractionPrompt(prompt);
        }

        protected override void InteractChild()
        {
            // Only interact if we're in a final decision window and currently peeking
            if (windowTrigger != null && windowTrigger.isFinalDecisionWindow)
            {
                windowTrigger.SelectPlanet(this); // Pass the entire PlanetSelector instead of just bool

                // Additional debug to verify the selection process
                Debug.Log($"WindowTrigger SelectPlanet called with planet: {planetName}");
            }
            else
            {
                Debug.LogWarning($"Cannot select planet - WindowTrigger: {windowTrigger?.name}, IsFinalDecision: {windowTrigger?.isFinalDecisionWindow}");
            }
        }

        // Optional: Add visual feedback when interaction is available
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && CanInteract)
            {
                // Visual feedback that planet can be interacted with
                // You could add highlighting, scale effect, etc. here
                Debug.Log($"{planetName} interaction available");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Remove visual feedback
                Debug.Log($"{planetName} interaction no longer available");
            }
        }

        // Method to enable/disable interaction based on peek state
        public void SetInteractionEnabled(bool enabled)
        {
            if (enabled)
                UnlockInteraction();
            else
                LockInteraction();
        }

        // Public getter for the planet's transform
        public Transform GetPlanetTransform()
        {
            return transform;
        }

        // Public getter for the decision target
        public Transform GetDecisionTarget()
        {
            return decisionTarget;
        }

        // Public getter for planet name
        public string GetPlanetName()
        {
            return planetName;
        }
    }
}