// AIDialogueManager.cs
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class AIDialogueManager : MonoBehaviour
{
    public static AIDialogueManager Instance { get; private set; }

    public UnityEvent<string, bool> OnNewMessage; // testo + isAISpeaking
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    [Range(0.01f, 0.5f)]
    public float typingSpeed = 0.05f;

    private Coroutine currentDialogueRoutine;
    private Coroutine typingRoutine;
    private bool isDialogueActive;

    [Header("Impostazioni Chiusura")]
    [SerializeField] private float endDelay = 2f; // Secondi prima della chiusura

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(AIDialogue dialogue)
    {
        if (isDialogueActive) return;

        if (dialogue == null || dialogue.messages.Count == 0) return;

        if (currentDialogueRoutine != null)
            StopCoroutine(currentDialogueRoutine);

        currentDialogueRoutine = StartCoroutine(RunDialogueRoutine(dialogue));
    }

    private IEnumerator RunDialogueRoutine(AIDialogue dialogue)
    {
        isDialogueActive = true;
        OnDialogueStart?.Invoke();

        foreach (var message in dialogue.messages)
        {
            if (typingRoutine != null)
                StopCoroutine(typingRoutine);

            typingRoutine = StartCoroutine(TypeText(message.text, message.isAISpeaking));

            yield return typingRoutine;
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(endDelay); // Aspetta prima di chiudere

        isDialogueActive = false;
        OnDialogueEnd?.Invoke();
    }

    private IEnumerator TypeText(string text, bool isAISpeaking)
    {
        string currentText = "";
        foreach (char letter in text.ToCharArray())
        {
            currentText += letter;
            OnNewMessage?.Invoke(currentText, isAISpeaking);
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public bool IsDialogueActive() => isDialogueActive;
}