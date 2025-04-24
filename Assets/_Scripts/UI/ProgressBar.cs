using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("Settings")]
        public Slider progressSlider;
        public float successThreshold = 100f;

        [Header("References")]
        public WindowPeekController peekController;

        private AsteroidSpawner asteroidSpawner;

        void Start()
        {
            // Automatically find the spawner if not assigned
            asteroidSpawner = FindAnyObjectByType<AsteroidSpawner>();

            // Safety check
            if (asteroidSpawner == null)
            {
                Debug.LogError("AsteroidSpawner reference is missing!");
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
            // Disable first to prevent multiple triggers
            this.enabled = false;

            // Force immediate destruction
            if (asteroidSpawner != null)
            {
                asteroidSpawner.DestroyAllActiveAsteroids();
            }

            peekController.EndPeek();
            progressSlider.value = 0f;

            // Re-enable after delay
            StartCoroutine(ReenableAfterDelay(1f));
        }


        private IEnumerator ReenableAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            this.enabled = true;
        }
    }
}
