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
        currentLineFullText = currentLine.lines.Count > 0 ? currentLine.lines[0] : "";

        dialogueText.PrepareLayout(currentLineFullText);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;

        foreach (string line in dialogueLine.lines)
        {
            string currentText = "";
            dialogueText.PrepareLayout(line);

            foreach (char letter in line.ToCharArray())
            {
                currentText += letter;
                dialogueText.UpdateText(currentText);

                if (typingSound != null && !typingSound.isPlaying)
                {
                    typingSound.Play();
                }

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;

        if (typingSound != null && typingSound.isPlaying)
        {
            typingSound.Stop();
        }

        ShowQuestionsIfAny();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueActive)
            {
                if (dialogueInteractor != null && dialogueInteractor.IsLookingAtNPC())
                {
                    dialogueInteractor.GetCurrentNPCDialogueTrigger().TriggerDialogue();
                }
            }
            else
            {
                HandleTextBubbleClick();
            }
        }
    }

    private void HandleTextBubbleClick()
    {
        // If we're at the end of dialogue (no more lines and not typing)
        if (!isTyping && lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Rest of the existing click handling
        if (currentLine != null && currentLine.questions != null && currentLine.questions.Count > 0)
        {
            return;
        }

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.PrepareLayout(currentLineFullText);
            dialogueText.UpdateText(currentLineFullText);
            isTyping = false;

            if (typingSound != null && typingSound.isPlaying)
            {
                typingSound.Stop();
            }

            ShowQuestionsIfAny();

            // If there are no more lines after skipping, end dialogue
            if (lines.Count == 0)
            {
                EndDialogue();
            }
        }
        else if (lines.Count > 0)
        {
            DisplayNextLine();
        }
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