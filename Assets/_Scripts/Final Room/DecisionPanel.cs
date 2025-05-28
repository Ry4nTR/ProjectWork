using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace ProjectWork.UI
{
    public class DecisionPanel : UI_Panel
    {
        [Header("Decision References")]
        [SerializeField] private Image overlayImage; // Full screen overlay
        [SerializeField] private TMP_Text confirmText; // Left side - Confirm text
        [SerializeField] private TMP_Text rejectText;  // Right side - Reject text

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Color overlayColor = new Color(0.8f, 0.8f, 0.8f, 0.7f); // Light gray with transparency

        private Coroutine currentFadeRoutine;

        protected override void Awake()
        {
            base.Awake();
            // Ensure we start hidden
            SetCanvasGroup(false);

            // Setup overlay color
            if (overlayImage != null)
            {
                overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
            }
        }

        public void ShowDecision(bool isEarth)
        {
            // Set the decision texts without the key letters
            confirmText.text = "CONFIRM";
            rejectText.text = "REJECT";

            // Enable the main canvas group first
            SetCanvasGroup(true);

            // Start fade in
            if (currentFadeRoutine != null)
                StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = StartCoroutine(FadePanel(true));
        }

        public void HideDecision()
        {
            // Start fade out
            if (currentFadeRoutine != null)
                StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = StartCoroutine(FadePanel(false));
        }

        public void ForceHide()
        {
            // Stop any running fade routine
            if (currentFadeRoutine != null)
            {
                StopCoroutine(currentFadeRoutine);
                currentFadeRoutine = null;
            }

            // Immediately hide everything
            SetCanvasGroup(false);

            // Reset overlay and text colors to transparent
            if (overlayImage != null)
                overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);

            if (confirmText != null)
            {
                Color confirmColor = confirmText.color;
                confirmText.color = new Color(confirmColor.r, confirmColor.g, confirmColor.b, 0f);

                // Also hide any child icons
                HideChildIcons(confirmText.transform);
            }

            if (rejectText != null)
            {
                Color rejectColor = rejectText.color;
                rejectText.color = new Color(rejectColor.r, rejectColor.g, rejectColor.b, 0f);

                // Also hide any child icons
                HideChildIcons(rejectText.transform);
            }
        }

        private void HideChildIcons(Transform parent)
        {
            // Hide any Image components in child objects (icons)
            Image[] childImages = parent.GetComponentsInChildren<Image>();
            foreach (Image img in childImages)
            {
                Color imgColor = img.color;
                img.color = new Color(imgColor.r, imgColor.g, imgColor.b, 0f);
            }
        }

        private void UpdateChildIconsAlpha(Transform parent, float alpha)
        {
            // Update alpha for any Image components in child objects (icons)
            Image[] childImages = parent.GetComponentsInChildren<Image>();
            foreach (Image img in childImages)
            {
                Color imgColor = img.color;
                img.color = new Color(imgColor.r, imgColor.g, imgColor.b, alpha);
            }
        }

        private IEnumerator FadePanel(bool show)
        {
            float targetAlpha = show ? overlayColor.a : 0f;
            float startAlpha = overlayImage.color.a;
            float elapsed = 0f;

            // Enable canvas group immediately when showing
            if (show)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }

            while (elapsed < fadeDuration)
            {
                float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                float textAlpha = currentAlpha / overlayColor.a;

                // Update overlay alpha
                overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, currentAlpha);

                // Update text alpha
                Color confirmColor = confirmText.color;
                Color rejectColor = rejectText.color;
                confirmText.color = new Color(confirmColor.r, confirmColor.g, confirmColor.b, textAlpha);
                rejectText.color = new Color(rejectColor.r, rejectColor.g, rejectColor.b, textAlpha);

                // Update child icon alpha
                UpdateChildIconsAlpha(confirmText.transform, textAlpha);
                UpdateChildIconsAlpha(rejectText.transform, textAlpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Set final values
            overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, targetAlpha);
            float finalTextAlpha = show ? 1f : 0f;

            Color finalConfirmColor = confirmText.color;
            Color finalRejectColor = rejectText.color;
            confirmText.color = new Color(finalConfirmColor.r, finalConfirmColor.g, finalConfirmColor.b, finalTextAlpha);
            rejectText.color = new Color(finalRejectColor.r, finalRejectColor.g, finalRejectColor.b, finalTextAlpha);

            // Set final alpha for child icons
            UpdateChildIconsAlpha(confirmText.transform, finalTextAlpha);
            UpdateChildIconsAlpha(rejectText.transform, finalTextAlpha);

            // Disable canvas group when hiding is complete
            if (!show)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            currentFadeRoutine = null;
        }
    }
}