using ProjectWork.UI;
using System;
using UnityEngine;
using System.Collections;

namespace ProjectWork
{
    public class FinalDecisionController : MonoBehaviour
    {
        [Header("References")]
        public WindowPeekController peekController;
        public DecisionPanel decisionPanel;

        [Header("Animation Settings")]
        public float planetApproachSpeed = 2f;
        public float planetApproachDelay = 0.5f; // Delay before planet starts moving

        private PlanetSelector selectedPlanetSelector;
        private Transform selectedPlanet;
        private Vector3 originalPlanetPosition;
        private bool decisionInProgress = false;
        private bool planetReached = false;
        private bool isEarthSelected = false;

        public static event Action<bool> OnFinalDecisionMade;

        private void Update()
        {
            // Decision input - only check when planet has reached position
            if (decisionInProgress && planetReached)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CompleteDecision(true); // Accept
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    CompleteDecision(false); // Reject
                }
            }
        }

        public void StartDecision(PlanetSelector planetSelector)
        {
            if (decisionInProgress)
            {
                return;
            }

            if (planetSelector == null)
            {
                Debug.LogError("Planet selector is null!");
                return;
            }

            Transform planet = planetSelector.GetPlanetTransform();
            Transform decisionTarget = planetSelector.GetDecisionTarget();

            if (planet == null)
            {
                Debug.LogError($"Planet transform is null for {planetSelector.GetPlanetName()}!");
                return;
            }

            if (decisionTarget == null)
            {
                Debug.LogError($"Decision target is null for {planetSelector.GetPlanetName()}! Please assign a decision target in the inspector.");
                return;
            }

            if (decisionPanel == null)
            {
                Debug.LogError("Decision panel is null!");
                return;
            }

            selectedPlanetSelector = planetSelector;
            selectedPlanet = planet;
            originalPlanetPosition = planet.position;
            isEarthSelected = planetSelector.isEarth;
            decisionInProgress = true;
            planetReached = false;

            peekController.LockExit();

            // Start the planet approach sequence
            StartCoroutine(PlanetApproachSequence());
        }

        private IEnumerator PlanetApproachSequence()
        {
            // Wait a moment before starting the approach
            yield return new WaitForSeconds(planetApproachDelay);

            // Get the target position from the planet selector's decision target
            Vector3 targetPosition = selectedPlanetSelector.GetDecisionTarget().position;

            // Move planet to target position
            float timeout = 10f; // Safety timeout
            float elapsed = 0f;

            while (Vector3.Distance(selectedPlanet.position, targetPosition) > 0.1f && elapsed < timeout)
            {
                Vector3 oldPos = selectedPlanet.position;
                selectedPlanet.position = Vector3.MoveTowards(
                    selectedPlanet.position,
                    targetPosition,
                    planetApproachSpeed * Time.deltaTime
                );

                elapsed += Time.deltaTime;

                yield return null;
            }

            if (elapsed >= timeout)
            {
                Debug.LogWarning("Planet approach timed out!");
            }

            // Snap to final position
            selectedPlanet.position = targetPosition;
            planetReached = true;

            // Show the decision overlay
            decisionPanel.ShowDecision(isEarthSelected);
        }

        private void CompleteDecision(bool accepted)
        {
            StartCoroutine(CompleteDecisionSequence(accepted));
        }

        private IEnumerator CompleteDecisionSequence(bool accepted)
        {
            // Hide the decision panel first
            decisionPanel.HideDecision();

            // Wait for panel to fade out
            yield return new WaitForSeconds(0.7f); // Slightly longer to ensure fade completes

            // Force hide the panel to ensure it's completely gone
            decisionPanel.ForceHide();

            // Return planet to original position if rejected
            if (!accepted && selectedPlanet != null)
            {
                while (Vector3.Distance(selectedPlanet.position, originalPlanetPosition) > 0.1f)
                {
                    selectedPlanet.position = Vector3.MoveTowards(
                        selectedPlanet.position,
                        originalPlanetPosition,
                        planetApproachSpeed * 2f * Time.deltaTime // Move back faster
                    );
                    yield return null;
                }
                selectedPlanet.position = originalPlanetPosition;
            }

            // Clean up
            decisionInProgress = false;
            planetReached = false;
            selectedPlanetSelector = null;
            selectedPlanet = null;
            peekController.UnlockExit();

            // Notify game of decision
            OnFinalDecisionMade?.Invoke(accepted);
        }
    }
}