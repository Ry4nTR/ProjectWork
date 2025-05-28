using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    /// <summary>
    /// Manages a progress bar puzzle where players must destroy asteroids to fill the bar.
    /// Inherits from Puzzle base class for puzzle completion functionality.
    /// </summary>
    public class ProgressBar : Puzzle
    {
        // Add this to the ProgressBar class variables
        [Header("Screen Management")]
        public PuzzleScreenManager screenManager;

        [Header("Progress Settings")]
        [Tooltip("The UI Slider representing the progress bar")]
        public Slider progressSlider;

        [Tooltip("Value needed to complete the puzzle")]
        public float successThreshold = 100f;

        [Tooltip("Base points added when hitting an asteroid")]
        [SerializeField] private float asteroidHitValue = 25f;

        [Tooltip("Base points deducted when asteroid reaches target")]
        [SerializeField] private float asteroidMissPenalty = 10f;

        [Header("Difficulty Settings")]
        [Tooltip("Multiplier for asteroid speed (higher = faster)")]
        [Range(0.5f, 3f)] public float difficultySpeedMultiplier = 1f;

        [Tooltip("Multiplier for spawn rate (higher = more frequent)")]
        [Range(0.5f, 2f)] public float difficultySpawnMultiplier = 1f;

        [Tooltip("Reduces point gain as progress increases (0 = no reduction)")]
        [Range(0f, 1f)] public float progressScalingFactor = 0.3f;

        [Header("References")]
        [Tooltip("Reference to the window peek controller")]
        public WindowPeekController peekController;

        [Tooltip("Reference to asteroid spawner (auto-assigned)")]
        [SerializeField] private AsteroidSpawner asteroidSpawner;

        private CanvasGroup canvasGroup;


        void Start()
        {
            asteroidSpawner = FindAnyObjectByType<AsteroidSpawner>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // Initialize screens if manager exists
            if (screenManager != null)
            {
                screenManager.InitializeScreens();
            }

            SetVisibility(false);

            if (asteroidSpawner == null)
            {
                Debug.LogError("AsteroidSpawner reference is missing!");
            }
            if (progressSlider == null)
            {
                Debug.LogError("Progress Slider reference is missing!");
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



        /// <summary>
        /// Handles window peek start event
        /// </summary>
        private void HandlePeekStarted(WindowTrigger window)
        {
            // Only show if this window has a progress bar
            SetVisibility(window.hasProgressBar);
            if (window.hasProgressBar)
            {
                progressSlider.value = 0f;
                UpdateAsteroidValues(); // Update values when starting
            }
        }

        /// <summary>
        /// Handles window peek end event
        /// </summary>
        private void HandlePeekEnded()
        {
            SetVisibility(false);
        }

        /// <summary>
        /// Sets the visibility of the progress bar UI
        /// </summary>
        private void SetVisibility(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1 : 0;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }
        }

        /// <summary>
        /// Updates asteroid values based on current progress for difficulty scaling
        /// </summary>
        private void UpdateAsteroidValues()
        {
            if (asteroidSpawner != null)
            {
                // Example: Get all active asteroids and update their values
                foreach (Asteroid asteroid in asteroidSpawner.activeAsteroids)
                {
                    // Could implement difficulty scaling here
                }
            }
        }

        /// <summary>
        /// Adds or subtracts progress from the bar
        /// </summary>
        public void AddProgress(float amount)
        {
            // Apply progress scaling based on current progress
            float scaledAmount = amount * (1 - (progressSlider.value / successThreshold * progressScalingFactor));

            progressSlider.value = Mathf.Clamp(progressSlider.value + scaledAmount, 0, successThreshold);

            if (progressSlider.value >= successThreshold)
            {
                PuzzleComplete();
            }
        }

        /// <summary>
        /// Handles puzzle completion logic
        /// </summary>
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