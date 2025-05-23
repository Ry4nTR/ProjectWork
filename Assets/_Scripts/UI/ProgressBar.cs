// ProgressBar.cs modifications
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    public class ProgressBar : Puzzle
    {
        [Header("Settings")]
        public Slider progressSlider;
        public float successThreshold = 100f;

        [Header("References")]
        public WindowPeekController peekController;

        private AsteroidSpawner asteroidSpawner;
        private CanvasGroup canvasGroup;

        void Start()
        {
            asteroidSpawner = FindAnyObjectByType<AsteroidSpawner>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // Start hidden
            SetVisibility(false);

            // Safety check
            if (asteroidSpawner == null)
            {
                Debug.LogError("AsteroidSpawner reference is missing!");
            }
        }

        private void OnEnable()
        {
            WindowTrigger.OnPeekStarted += HandlePeekStarted;
            WindowTrigger.OnPeekEnded += HandlePeekEnded;
        }

        private void OnDisable()
        {
            WindowTrigger.OnPeekStarted -= HandlePeekStarted;
            WindowTrigger.OnPeekEnded -= HandlePeekEnded;
        }

        private void HandlePeekStarted(WindowTrigger window)
        {
            // Only show if this window has a progress bar
            SetVisibility(window.hasProgressBar);
            if (window.hasProgressBar)
            {
                progressSlider.value = 0f;
            }
        }

        private void HandlePeekEnded()
        {
            SetVisibility(false);
        }

        private void SetVisibility(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1 : 0;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }
        }

        public void AddProgress(float amount)
        {
            progressSlider.value = Mathf.Clamp(progressSlider.value + amount, 0, successThreshold);

            if (progressSlider.value >= successThreshold)
            {
                PuzzleComplete();
            }
        }

        private void PuzzleComplete()
        {
            if (asteroidSpawner != null)
            {
                asteroidSpawner.DestroyAllActiveAsteroids();
            }

            peekController.EndPeek();
            progressSlider.value = 0f;

            InvokePuzzleCompletedEvent();
        }
    }
}