// AIDialogueUI.cs modificato
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectWork.UI;
using System.Collections; // Aggiungi questo namespace


namespace ProjectWork
{
    public class AIDialogueUI : UI_Panel // Ora eredita da UI_Panel
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Image speakerIcon;
        [SerializeField] private Sprite aiIcon;
        [SerializeField] private Sprite playerIcon;

        private RectTransform backgroundRect;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.5f;

        // Configuration parameters
        [Header("Text Settings")]
        [SerializeField] private float verticalTextPadding = 30f;
        [SerializeField] private float baseSize = 200f;

        protected override void Awake()
        {
            // Non chiamare base.Awake() per evitare l'inizializzazione automatica
            base.Awake();
            backgroundRect = dialogueText.transform.parent.GetComponent<RectTransform>();
        }

        private void Start()
        {
            AIDialogueManager.OnNewMessage += UpdateDialogue;
            AIDialogueManager.OnDialogueStart += ShowDialogue;
            AIDialogueManager.OnDialogueEnd += HideDialogue;
        }

        private void OnDestroy()
        {
            AIDialogueManager.OnNewMessage -= UpdateDialogue;
            AIDialogueManager.OnDialogueStart -= ShowDialogue;
            AIDialogueManager.OnDialogueEnd -= HideDialogue;
        }

        private void UpdateDialogue(string text, bool isAISpeaking)
        {
            dialogueText.text = text;
            dialogueText.ForceMeshUpdate(); // Forza l'aggiornamento del testo per il rendering corretto
            Vector2 textSize = dialogueText.GetRenderedValues(false);

            // Imposta la dimensione del background in base al testo
            backgroundRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, baseSize + textSize.y + verticalTextPadding); // Aggiungi padding

            speakerIcon.sprite = isAISpeaking ? aiIcon : playerIcon;
        }

        private void ShowDialogue()
        {
            StartCoroutine(FadeIn());
        }

        private void HideDialogue()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetCanvasGroup(true); // Usa il metodo della classe base
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetCanvasGroup(false); // Usa il metodo della classe base
        }
    }
}