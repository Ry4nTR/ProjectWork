using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Queue<DialogueLine> lines;
    private List<QuestionText> questionBoxes = new List<QuestionText>();

    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;

    private DialogueText dialogueText;
    private Transform currentQuestionBoxContainer;
    private bool isTyping = false;
    private string currentLineFullText;
    private bool isShowingCompletedDialogue = false;
    private int lineIndexInCurrentDialogueLine = 0;


    [Header("NPC Interaction")]
    [SerializeField] private DialogueInteractor dialogueInteractor;

    public GameObject questionBoxPrefab;
    private Dictionary<string, DialogueLine> responseLookup;
    private DialogueLine currentLine;
    public AudioSource typingSound;

    public bool IsDialogueActive() => isDialogueActive;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue, DialogueText textBubble, Transform questionBoxContainer)
    {
        // Toggle dialogue if already active with same text bubble
        if (isDialogueActive && dialogueText == textBubble)
        {
            EndDialogue();
            return;
        }

        isDialogueActive = true;
        dialogueText = textBubble;
        currentQuestionBoxContainer = questionBoxContainer;
        dialogueText.gameObject.SetActive(true);

        lines.Clear();
        responseLookup = new Dictionary<string, DialogueLine>();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            if (!string.IsNullOrEmpty(dialogueLine.lineId))
            {
                responseLookup[dialogueLine.lineId] = dialogueLine;
            }
            lines.Enqueue(dialogueLine);
        }

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();
        lineIndexInCurrentDialogueLine = 0;
        currentLineFullText = currentLine.lines[lineIndexInCurrentDialogueLine];
        dialogueText.PrepareLayout(currentLineFullText);

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.lines[lineIndexInCurrentDialogueLine]));
    }


    IEnumerator TypeSentence(string line)
    {
        isTyping = true;
        string currentText = "";

        dialogueText.PrepareLayout(line);

        foreach (char letter in line.ToCharArray())
        {
            currentText += letter;
            dialogueText.UpdateText(currentText);

            if (typingSound != null && !typingSound.isPlaying)
                typingSound.Play();

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (typingSound != null && typingSound.isPlaying)
            typingSound.Stop();
    }


    void EndDialogue()
    {
        if (!isDialogueActive) return;

        isDialogueActive = false;
        isShowingCompletedDialogue = false;
        ClearQuestions();

        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
    }

    private void HandleTextBubbleClick()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.PrepareLayout(currentLineFullText);
            dialogueText.UpdateText(currentLineFullText);
            isTyping = false;

            if (typingSound != null && typingSound.isPlaying)
                typingSound.Stop();

            return;
        }

        // Vai alla prossima riga della stessa DialogueLine
        lineIndexInCurrentDialogueLine++;

        if (currentLine != null && lineIndexInCurrentDialogueLine < currentLine.lines.Count)
        {
            currentLineFullText = currentLine.lines[lineIndexInCurrentDialogueLine];
            dialogueText.PrepareLayout(currentLineFullText);

            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentLine.lines[lineIndexInCurrentDialogueLine]));
            return;
        }

        // Se ci sono domande dopo l'ultima linea della DialogueLine, mostrale
        if (currentLine != null && currentLine.questions != null && currentLine.questions.Count > 0)
        {
            ShowQuestions(currentLine.questions);
            return;
        }

        // Se ci sono altre DialogueLine
        if (lines.Count > 0)
        {
            DisplayNextLine();
            return;
        }

        // Altrimenti fine dialogo
        EndDialogue();
    }




    public void HandleExternalClick()
    {
        HandleTextBubbleClick();
    }


    private void ShowQuestionsIfAny()
    {
        if (currentLine != null && currentLine.questions != null && currentLine.questions.Count > 0)
        {
            ShowQuestions(currentLine.questions);
        }
    }

    private void ShowQuestions(List<DialogueQuestion> questions)
    {
        ClearQuestions();
        float verticalOffset = 0f;
        float spacing = .6f;

        for (int i = 0; i < questions.Count; i++)
        {
            GameObject questionBoxObj = Instantiate(questionBoxPrefab, currentQuestionBoxContainer);
            QuestionText questionBox = questionBoxObj.GetComponent<QuestionText>();

            if (responseLookup.TryGetValue(questions[i].responseId, out DialogueLine response))
            {
                questionBox.Setup(questions[i].question, response);
            }
            else
            {
                Debug.LogError($"Response not found for ID: {questions[i].responseId}");
            }

            questionBoxObj.transform.localPosition = new Vector3(0, verticalOffset, 0);
            verticalOffset -= spacing;
            questionBoxes.Add(questionBox);
        }
    }

    private void ClearQuestions()
    {
        foreach (QuestionText questionBox in questionBoxes)
        {
            Destroy(questionBox.gameObject);
        }
        questionBoxes.Clear();
    }

    public void OnQuestionSelected(DialogueLine response)
    {
        ClearQuestions();

        if (responseLookup.TryGetValue(response.lineId, out DialogueLine responseLine))
        {
            lines.Clear();
            foreach (string line in responseLine.lines)
            {
                DialogueLine newLine = new DialogueLine
                {
                    lines = new List<string> { line },
                    questions = responseLine.questions
                };
                lines.Enqueue(newLine);
            }

            DisplayNextLine();
        }
        else
        {
            Debug.LogError($"Response not found for ID: {response.lineId}");
        }
    }
}