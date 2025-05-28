using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace ProjectWork
{
    [RequireComponent(typeof(Image))]
    public class BlackScreenTextController : MonoBehaviour
    {
        public static BlackScreenTextController Instance { get; private set; } = null;

        public static event Action OnBlackScreenFullActivated = delegate { };
        public static event Action OnBlackScreenTextStarted = delegate { };
        public static event Action OnBlackScreenTextFinished = delegate { };

        private Image blackBackground;
        private TextMeshProUGUI dialogueText;
        [SerializeField] private BlackScreenData initialBlackScreenData = null;

        void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                SceneManager.sceneLoaded += StartBlackScreen;
                blackBackground = GetComponent<Image>();
                dialogueText = GetComponentInChildren<TextMeshProUGUI>();

                SetAlpha(0f, 0f, false);
            }       
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= StartBlackScreen;
        }

        private void StartBlackScreen(Scene arg0, LoadSceneMode arg1)
        {
            ActivateBlackScreen(initialBlackScreenData);
        }

        public void ActivateBlackScreen(BlackScreenData blackScreenData)
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(blackScreenData));
        }

        IEnumerator FadeRoutine(BlackScreenData blackScreenData)
        {
            dialogueText.text = blackScreenData.TextToShow;
            blackBackground.enabled = true;

            OnBlackScreenTextStarted?.Invoke();

            if (blackScreenData.FadeInSettings.UseFade)
            {
                yield return Fade(0f, 1f, blackScreenData.FadeInSettings.FadeDuration, blackScreenData.FadeInSettings.FadeOnlyText);
            }
            else
            {
                SetAlpha(1f, 1f, true);
            }
            yield return new WaitForSeconds(blackScreenData.TextStayDuration);

            if(blackScreenData.FadeOutSettings.UseFade)
            {
                yield return Fade(1f, 0f, blackScreenData.FadeOutSettings.FadeDuration, blackScreenData.FadeOutSettings.FadeOnlyText);
            }
            else
            {
                SetAlpha(0f, 0f, false);
            }
            OnBlackScreenTextFinished?.Invoke();

            blackBackground.enabled = false;
        }

        IEnumerator Fade(float from, float to, float fadeDuration, bool fadeOnlyText)
        {
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
                SetAlpha(fadeOnlyText ? 1 : alpha, alpha, false);
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetAlpha(to, to, true);
        }

        void SetAlpha(float bgAlpha, float textAlpha, bool canInvokeEvent)
        {
            Color bgColor = blackBackground.color;
            bgColor.a = bgAlpha;
            blackBackground.color = bgColor;

            Color txtColor = dialogueText.color;
            txtColor.a = textAlpha;
            dialogueText.color = txtColor;

            if(canInvokeEvent && textAlpha == 1f)
            {
                OnBlackScreenFullActivated?.Invoke();
            }
        }
    }
}

