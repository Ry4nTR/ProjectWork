using System.Collections;
using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(Light))]
    public class EnvironmentLight : MonoBehaviour
    {
        [Header("Light Color Settings")]
        [SerializeField] private Color normalLightColor = Color.white;
        [SerializeField] private Color emergencyLightColor = Color.red;

        [Header("Blink Settings")]
        [SerializeField] private float blinkSpeed = 1f;          // How fast the intensity blinks
        private const float MIN_INTENSITY = 0f;      // Minimum brightness
        private float maxIntensity;      // Maximum brightness

        private Light myLight;
        private Coroutine blinkingCoroutine;

        protected void Awake()
        {
            myLight = GetComponent<Light>();
            TutorialTaskChecker.OnDayPassed += TrySetEmergencyLights;
        }

        private void Start()
        {
            maxIntensity = myLight.intensity; // Store the initial intensity
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= TrySetEmergencyLights;
        }

        private void TrySetEmergencyLights(bool isEmergency)
        {
            myLight.color = isEmergency ? emergencyLightColor : normalLightColor;

            if (isEmergency)
            {
                if (blinkingCoroutine == null)
                    blinkingCoroutine = StartCoroutine(BlinkingLightCoroutine());
            }
            else
            {
                if (blinkingCoroutine != null)
                {
                    StopCoroutine(blinkingCoroutine);
                    blinkingCoroutine = null;
                    myLight.intensity = maxIntensity; // Reset to default
                }
            }
        }

        private IEnumerator BlinkingLightCoroutine()
        {
            while (true)
            {
                float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
                myLight.intensity = Mathf.Lerp(MIN_INTENSITY, maxIntensity, t);
                yield return null;
            }
        }
    }
}
