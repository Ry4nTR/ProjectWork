// AIDialogueUI.cs modificato
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectWork.UI;
using System.Collections; // Aggiungi questo namespace

public class AIDialogueUI : UI_Panel // Ora eredita da UI_Panel
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image speakerIcon;
    [SerializeField] private Sprite aiIcon;
    [SerializeField] private Sprite playerIcon;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    protected override void Awake()
    {
        // Non chiamare base.Awake() per evitare l'inizializzazione automatica
        canvasGroup = GetComponent<CanvasGroup>();

        AIDialogueManager.Instance.OnNewMessage.AddListener(UpdateDialogue);
        AIDialogueManager.Instance.OnDialogueStart.AddListener(ShowDialogue);
        AIDialogueManager.Instance.OnDialogueEnd.AddListener(HideDialogue);
    }

    private void UpdateDialogue(string text, bool isAISpeaking)
    {
        dialogueText.text = text;
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