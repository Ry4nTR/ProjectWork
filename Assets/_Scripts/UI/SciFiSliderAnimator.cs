using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    /// <summary>
    /// Animates a sci-fi styled slider with smooth transitions and random value changes.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class SciFiSliderAnimator : MonoBehaviour
    {
        [Header("Slider Animation")]
        [Tooltip("Speed at which the slider value interpolates to the target.")]
        [SerializeField] private float animationSpeed = 5f;

        [Header("Random Value Timing")]
        [Tooltip("Minimum delay before the next random value.")]
        [SerializeField] private float minChangeInterval = 1f;
        [Tooltip("Maximum delay before the next random value.")]
        [SerializeField] private float maxChangeInterval = 3f;

        private Slider slider;
        private float targetValue;
        private float nextChangeTime;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            targetValue = slider.value;
            ScheduleNextChange();
        }

        private void Update()
        {
            // Smooth transition toward target value
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.unscaledDeltaTime * animationSpeed);

            // Trigger a new random value if it's time
            if (Time.unscaledTime >= nextChangeTime)
            {
                SetSliderValue(Random.Range(0f, 1f));
                ScheduleNextChange();
            }
        }

        private void ScheduleNextChange()
        {
            nextChangeTime = Time.unscaledTime + Random.Range(minChangeInterval, maxChangeInterval);
        }

        /// <summary>
        /// Sets the target slider value.
        /// </summary>
        /// <param name="value">Normalized slider value (0 to 1).</param>
        public void SetSliderValue(float value)
        {
            targetValue = Mathf.Clamp01(value);
        }
    }
}
