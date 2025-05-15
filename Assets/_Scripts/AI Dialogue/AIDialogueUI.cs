// AIDialogueUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AIDialogueUI : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image speakerIcon; // Componente Image per l'icona

    [Header("Sprites")]
    [SerializeField] private Sprite aiIcon; // Sprite per l'AI
    [SerializeField] private Sprite playerIcon; // Sprite per il player

    [Header("Impostazioni Scomparsa")]
    [SerializeField] private float fadeOutDuration = 0.5f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        AIDialogueManager.Instance.OnNewMessage.AddListener(UpdateDialogue);
        AIDialogueManager.Instance.OnDialogueStart.AddListener(ShowDialogue);
        AIDialogueManager.Instance.OnDialogueEnd.AddListener(HideDialogue);
    }

    private void ShowDialogue()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideDialogue()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    private void UpdateDialogue(string text, bool isAISpeaking)
    {
        dialogueText.text = text;
        speakerIcon.CrossFadeAlpha(0, 0.1f, false); // Fade out
        speakerIcon.sprite = isAISpeaking ? aiIcon : playerIcon;
        speakerIcon.CrossFadeAlpha(1, 0.3f, false); // Fade in
    }
}