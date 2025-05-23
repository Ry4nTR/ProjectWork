using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    public class FireExtinguisherScreenStatus : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private Color availableColor = Color.green;
        [SerializeField] private Color solvedColor = Color.blue;
        [SerializeField] private Color emptyColor = Color.red;
        [SerializeField] private string statusTextFormat = "Refilling... {00%}";
        [SerializeField] private string fullyChargedStatusText = "Available";
        [SerializeField] private string solvedIssueStatusText = "Good job!";

        private const float MAX_VALUE = 1f;

        [Header("References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private TMPro.TextMeshProUGUI statusText;
        [SerializeField] private FireExtinguisher connectedFireExtinguisher;
        private Slider refillingSlider;

        private void Awake()
        {
            refillingSlider = GetComponentInChildren<Slider>();

            connectedFireExtinguisher.OnRefillTimerChanged += UpdateScreen;
            connectedFireExtinguisher.OnRefillTimerFinished += SetAvailableScreen;
            connectedFireExtinguisher.OnRefillTimerStopped += SetSolvedScreen;
        }

        private void OnDestroy()
        {
            connectedFireExtinguisher.OnRefillTimerChanged -= UpdateScreen;
            connectedFireExtinguisher.OnRefillTimerFinished -= SetAvailableScreen;
            connectedFireExtinguisher.OnRefillTimerStopped -= SetSolvedScreen;
        }

        private void SetAvailableScreen()
        {
            UpdateScreen(MAX_VALUE);
        }

        private void SetSolvedScreen()
        {
            backgroundImage.color = solvedColor;
            fillImage.color = solvedColor;
            statusText.text = solvedIssueStatusText;
            refillingSlider.value = MAX_VALUE;
            refillingSlider.gameObject.SetActive(false);
        }

        private void UpdateScreen(float value)
        {
            float convertedValue = value == MAX_VALUE ? value : 1 - value;
            backgroundImage.color = Color.Lerp(emptyColor, availableColor, convertedValue);
            UpdateSlider(convertedValue);
            UpdateText(convertedValue);
        }

        private void UpdateSlider(float value)
        {
            Color newColor = Color.Lerp(emptyColor, availableColor, value);
            refillingSlider.value = value;
            refillingSlider.colors = new ColorBlock
            {
                normalColor = newColor,
                highlightedColor = newColor,
                pressedColor = newColor,
                selectedColor = newColor,
                disabledColor = newColor,
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
        }

        private void UpdateText(float value)
        {
            statusText.text = value != MAX_VALUE
                ? string.Format(statusTextFormat, Mathf.RoundToInt(value * 100f))
                : fullyChargedStatusText;
        }
    }
}