using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

namespace ProjectWork
{
    /// <summary>
    /// Displays black screen with text for transitions or narrative moments.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class BlackScreenTextController : MonoBehaviour
    {
        public static BlackScreenTextController Instance { get; private set; } = null;

        public static event Action OnBlackScreenTextStarted = delegate { };
        public static event Action OnBlackScreenTextFinished = delegate { };

        private Image blackBackground;
        private TextMeshProUGUI dialogueText;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float textStayDuration = 2f;

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
                blackBackground = GetComponent<Image>();
                dialogueText = GetComponentInChildren<TextMeshProUGUI>();

                SetAlpha(0f, 0f);
            }       
        }
        private void Start()
        {
            //ActivateBlackScreen("Starting text");
        }

        public void ActivateBlackScreen(string text)
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(text));
        }

        IEnumerator FadeRoutine(string message)
        {
            dialogueText.text = message;
            blackBackground.enabled = true;

            OnBlackScreenTextStarted?.Invoke();
            yield return Fade(0f, 1f);
            yield return new WaitForSeconds(textStayDuration);
            yield return Fade(1f, 0f);
            OnBlackScreenTextFinished?.Invoke();

            blackBackground.enabled = false;
        }

        IEnumerator Fade(float from, float to)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
                SetAlpha(alpha, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetAlpha(to, to);
        }

        void SetAlpha(float bgAlpha, float textAlpha)
        {
            Color bgColor = blackBackground.color;
            bgColor.a = bgAlpha;
            blackBackground.color = bgColor;

            Color txtColor = dialogueText.color;
            txtColor.a = textAlpha;
            dialogueText.color = txtColor;
        }
    }
}

