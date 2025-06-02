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
        public float planetApproachDelay = 0.5f;

        private PlanetSelector selectedPlanetSelector;
        private Transform selectedPlanet;
        private Vector3 originalPlanetPosition;
        private bool decisionInProgress = false;
        private bool planetReached = false;
        private bool isEarthSelected = false;

        public static event Action<bool> OnFinalDecisionMade;

        private void Update()
        {
            if (decisionInProgress && planetReached)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CompleteDecision(true);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    CompleteDecision(false);
                }
            }
        }

        public void StartDecision(PlanetSelector planetSelector)
        {
            if (decisionInProgress || planetSelector == null)
                return;

            Transform planet = planetSelector.GetPlanetTransform();
            Transform decisionTarget = planetSelector.GetDecisionTarget();

            if (planet == null || decisionTarget == null || decisionPanel == null)
                return;

            selectedPlanetSelector = planetSelector;
            selectedPlanet = planet;
            originalPlanetPosition = planet.position;
            isEarthSelected = planetSelector.isEarth;
            decisionInProgress = true;
            planetReached = false;

            peekController.LockExit();
            StartCoroutine(PlanetApproachSequence());
        }

        private IEnumerator PlanetApproachSequence()
        {
            yield return new WaitForSeconds(planetApproachDelay);

            Vector3 targetPosition = selectedPlanetSelector.GetDecisionTarget().position;
            float timeout = 10f;
            float elapsed = 0f;

            while (Vector3.Distance(selectedPlanet.position, targetPosition) > 0.1f && elapsed < timeout)
            {
                selectedPlanet.position = Vector3.MoveTowards(
                    selectedPlanet.position,
                    targetPosition,
                    planetApproachSpeed * Time.deltaTime
                );
                elapsed += Time.deltaTime;
                yield return null;
            }

            selectedPlanet.position = targetPosition;
            planetReached = true;
            decisionPanel.ShowDecision(isEarthSelected);
        }

        private void CompleteDecision(bool accepted)
        {
            if (accepted && decisionPanel != null)
            {
                decisionPanel.ConfirmDecision();
            }

            StartCoroutine(CompleteDecisionSequence(accepted));
        }

        private IEnumerator CompleteDecisionSequence(bool accepted)
        {
            decisionPanel.HideDecision();
            yield return new WaitForSeconds(0.7f);
            decisionPanel.HideDecision();

            if (!accepted && selectedPlanet != null)
            {
                while (Vector3.Distance(selectedPlanet.position, originalPlanetPosition) > 0.1f)
                {
                    selectedPlanet.position = Vector3.MoveTowards(
                        selectedPlanet.position,
                        originalPlanetPosition,
                        planetApproachSpeed * 2f * Time.deltaTime
                    );
                    yield return null;
                }
                selectedPlanet.position = originalPlanetPosition;
            }

            decisionInProgress = false;
            planetReached = false;
            selectedPlanetSelector = null;
            selectedPlanet = null;
            peekController.UnlockExit();
            OnFinalDecisionMade?.Invoke(accepted);
        }
    }
}