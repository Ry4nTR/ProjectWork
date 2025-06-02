using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork.UI
{
    public class DecisionPanel : UI_Panel
    {
        [Header("Decision References")]
        [SerializeField] private Image overlayImage;
        [SerializeField] private TMP_Text confirmText;
        [SerializeField] private TMP_Text rejectText;

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Color overlayColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);

        [Header("Black Screen Settings")]
        [SerializeField] private BlackScreenData confirmBlackScreenData;
        [SerializeField] private bool showBlackScreenOnConfirm = true;

        private Coroutine currentFadeRoutine;
        public static event Action OnDecisionConfirmed;

        protected override void Awake()
        {
            base.Awake();
            SetCanvasGroup(false);
            overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
        }

        public void ShowDecision(bool isEarth)
        {
            confirmText.text = "CONFIRM";
            rejectText.text = "REJECT";
            SetCanvasGroup(true);

            if (currentFadeRoutine != null)
                StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = StartCoroutine(FadePanel(true));
        }

        public void ConfirmDecision()
        {
            if (showBlackScreenOnConfirm && confirmBlackScreenData != null)
            {
                BlackScreenTextController.Instance.ActivateBlackScreen(confirmBlackScreenData);
            }

            OnDecisionConfirmed?.Invoke();
            HideDecision();
        }

        public void OnConfirmButtonPressed()
        {
            ConfirmDecision();
        }

        public void HideDecision()
        {
            if (currentFadeRoutine != null)
                StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = StartCoroutine(FadePanel(false));
        }

        private IEnumerator FadePanel(bool show)
        {
            float targetAlpha = show ? overlayColor.a : 0f;
            float startAlpha = overlayImage.color.a;
            float elapsed = 0f;

            if (show)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }

            while (elapsed < fadeDuration)
            {
                float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                float textAlpha = currentAlpha / overlayColor.a;

                overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, currentAlpha);
                confirmText.color = new Color(confirmText.color.r, confirmText.color.g, confirmText.color.b, textAlpha);
                rejectText.color = new Color(rejectText.color.r, rejectText.color.g, rejectText.color.b, textAlpha);

                UpdateChildIconsAlpha(confirmText.transform, textAlpha);
                UpdateChildIconsAlpha(rejectText.transform, textAlpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, targetAlpha);
            float finalTextAlpha = show ? 1f : 0f;

            confirmText.color = new Color(confirmText.color.r, confirmText.color.g, confirmText.color.b, finalTextAlpha);
            rejectText.color = new Color(rejectText.color.r, rejectText.color.g, rejectText.color.b, finalTextAlpha);

            UpdateChildIconsAlpha(confirmText.transform, finalTextAlpha);
            UpdateChildIconsAlpha(rejectText.transform, finalTextAlpha);

            if (!show)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            currentFadeRoutine = null;
        }

        private void UpdateChildIconsAlpha(Transform parent, float alpha)
        {
            foreach (Image img in parent.GetComponentsInChildren<Image>())
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            }
        }
    }
}