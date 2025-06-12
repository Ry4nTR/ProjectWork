using System.Collections;
using UnityEngine;

namespace ProjectWork
{
    /// <summary>
    /// Rotates the object in discrete, constant-speed segments with clear transitions. Good for sci-fi UI effects.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SciFiCircleRotator : MonoBehaviour
    {
        [Tooltip("Degrees per second for constant rotation.")]
        [SerializeField] private float rotationSpeed = 90f;

        [Tooltip("Minimum delay before selecting a new target angle.")]
        [SerializeField] private float minPauseTime = 0.2f;

        [Tooltip("Maximum delay before selecting a new target angle.")]
        [SerializeField] private float maxPauseTime = 1f;

        [Tooltip("Minimum degrees to rotate per segment.")]
        [SerializeField] private float minRotationDelta = 30f;

        [Tooltip("Maximum degrees to rotate per segment.")]
        [SerializeField] private float maxRotationDelta = 180f;

        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            StartCoroutine(RotationRoutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator RotationRoutine()
        {
            while (true)
            {
                // Pick random rotation delta and direction
                float delta = Random.Range(minRotationDelta, maxRotationDelta);
                delta *= Random.value < 0.5f ? -1f : 1f;

                float rotated = 0f;
                float total = Mathf.Abs(delta);

                while (rotated < total)
                {
                    float step = rotationSpeed * Time.unscaledDeltaTime;
                    float rotationStep = Mathf.Min(step, total - rotated);
                    rotated += rotationStep;

                    rect.Rotate(0f, 0f, rotationStep * Mathf.Sign(delta));
                    yield return null;
                }

                // Pause before the next segment
                float wait = Random.Range(minPauseTime, maxPauseTime);
                yield return new WaitForSecondsRealtime(wait);
            }
        }
    }
}
