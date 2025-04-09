using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace ITSProjectWork
{
    /// <summary>
    /// Displays black screen with text for transitions or narrative moments.
    /// </summary>
    public class BlackScreenTextController : MonoBehaviour
    {
        [SerializeField] Image blackBackground;
        [SerializeField] TextMeshProUGUI dialogueText;
        [SerializeField] float fadeDuration = 1f;
        [SerializeField] float textStayDuration = 2f;

        void Awake()
        {
            if (blackBackground == null) blackBackground = GetComponentInChildren<Image>();
            if (dialogueText == null) dialogueText = GetComponentInChildren<TextMeshProUGUI>();

            SetAlpha(0f, 0f);
        }

        public void ShowText(string text)
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(text));
        }

        IEnumerator FadeRoutine(string message)
        {
            dialogueText.text = message;
            yield return Fade(0f, 1f);
            yield return new WaitForSeconds(textStayDuration);
            yield return Fade(1f, 0f);
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
            var bgColor = blackBackground.color;
            bgColor.a = bgAlpha;
            blackBackground.color = bgColor;

            var txtColor = dialogueText.color;
            txtColor.a = textAlpha;
            dialogueText.color = txtColor;
        }
    }
}

