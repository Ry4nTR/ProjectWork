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
            peekController.EndPeek();
            progressSlider.value = 0f;
        }
    }
}