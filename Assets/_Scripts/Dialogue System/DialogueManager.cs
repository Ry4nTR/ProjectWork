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

    public float typingSpeed = 0.75f;
    public float lineDelay = 1f; // Delay after a line is fully displayed before proceeding to the next line

    private DialogueText dialogueText;
    private Transform currentQuestionBoxContainer; // Store the current question box container
    private bool isTyping = false;
    private string currentLineFullText;
    private float interactionDistance = 5f;

    public GameObject questionBoxPrefab;

    private Dictionary<string, DialogueLine> responseLookup;

    private DialogueLine currentLine; // Store the current line being displayed

    // Add an AudioSource for the typing sound
    public AudioSource typingSound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue, DialogueText textBubble, Transform questionBoxContainer)
    {
        isDialogueActive = true;

        dialogueText = textBubble;
        currentQuestionBoxContainer = questionBoxContainer; // Set the current question box container
        dialogueText.gameObject.SetActive(true);

        lines.Clear();

        // Initialize the response lookup dictionary
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

        currentLine = lines.Dequeue(); // Store the current line

        // Store the full text of the current line
        currentLineFullText = currentLine.lines.Count > 0 ? currentLine.lines[0] : "";

        dialogueText.Setup(""); // Clear the text bubble

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;

        foreach (string line in dialogueLine.lines)
        {
            string currentText = "";
            foreach (char letter in line.ToCharArray())
            {
                currentText += letter;
                dialogueText.Setup(currentText);

                // Play the typing sound for each character
                if (typingSound != null && !typingSound.isPlaying)
                {
                    typingSound.Play();
                }

                yield return new WaitForSeconds(typingSpeed);
            }

            // Wait for a short delay before proceeding to the next line
            yield return new WaitForSeconds(lineDelay); // Adjust the delay as needed
        }

        isTyping = false; // Typing is complete

        // Stop the typing sound when typing is complete
        if (typingSound != null && typingSound.isPlaying)
        {
            typingSound.Stop();
        }

        // Show questions only after all lines are displayed
        ShowQuestionsIfAny();

        // If there are no questions, proceed to the next line after a delay
        if (currentLine.questions == null || currentLine.questions.Count == 0)
        {
            StartCoroutine(ProceedToNextLineAfterDelay());
        }
    }

    void EndDialogue()
    {
        if (!isDialogueActive) return; // Prevent multiple calls to EndDialogue

        isDialogueActive = false;

        // Check if currentTextBubble is not null before accessing it
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false); // Hide the text bubble
            dialogueText = null; // Clear the reference
        }

        currentQuestionBoxContainer = null; // Clear the question box container reference

        // Clear all question boxes
        ClearQuestions();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Check if we're not already in dialogue or if we can proceed
            if (!isDialogueActive)
            {
                // Raycast to detect NPC
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, interactionDistance)) // Use your desired interaction distance
                {
                    DialogueTrigger trigger = hit.collider.GetComponent<DialogueTrigger>();
                    if (trigger != null)
                    {
                        trigger.TriggerDialogue();
                    }
                }
            }
            else
            {
                // This handles continuing dialogue when E is pressed
                HandleTextBubbleClick();
            }
        }
    }

    private void HandleTextBubbleClick()
    {
        // Do nothing if questions are currently displayed
        if (currentLine != null && currentLine.questions != null && currentLine.questions.Count > 0)
        {
            return;
        }

        if (isTyping)
        {
            // If typing is in progress, skip to the end of the current line
            StopAllCoroutines(); // Stop the typing coroutine
            dialogueText.Setup(currentLineFullText); // Display the full text immediately
            isTyping = false; // Mark typing as complete

            // Stop the typing sound when skipping
            if (typingSound != null && typingSound.isPlaying)
            {
                typingSound.Stop();
            }

            // Show questions if the current line has any
            ShowQuestionsIfAny();

            // If there are no questions, proceed to the next line after a delay
            if (currentLine.questions == null || currentLine.questions.Count == 0)
            {
                StartCoroutine(ProceedToNextLineAfterDelay());
            }
        }
        else
        {
            // If typing is complete, proceed to the next line immediately
            DisplayNextLine();
        }
    }

    private IEnumerator ProceedToNextLineAfterDelay()
    {
        // Wait for the same delay as in TypeSentence before proceeding to the next line
        yield return new WaitForSeconds(lineDelay);

        // Proceed to the next line
        DisplayNextLine();
    }

    private void ShowQuestionsIfAny()
    {
        // Show questions only if the current line has questions
        if (currentLine != null && currentLine.questions != null && currentLine.questions.Count > 0)
        {
            ShowQuestions(currentLine.questions);
        }
    }

    private void ShowQuestions(List<DialogueQuestion> questions)
    {
        ClearQuestions(); // Clear any existing question boxes

        float verticalOffset = 0f; // Initial vertical offset
        float spacing = .6f; // Space between question boxes

        for (int i = 0; i < questions.Count; i++)
        {
            // Instantiate the question box in the NPC's question box container
            GameObject questionBoxObj = Instantiate(questionBoxPrefab, currentQuestionBoxContainer);
            QuestionText questionBox = questionBoxObj.GetComponent<QuestionText>();

            // Set the question text and response
            if (responseLookup.TryGetValue(questions[i].responseId, out DialogueLine response))
            {
                questionBox.Setup(questions[i].question, response);
            }
            else
            {
                Debug.LogError($"Response not found for ID: {questions[i].responseId}");
            }

            // Position the question box
            questionBoxObj.transform.localPosition = new Vector3(0, verticalOffset, 0);
            verticalOffset -= spacing; // Move the next question box down

            // Add the question box to the list
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
        ClearQuestions(); // Clear all question boxes

        // Look up the response using the responseId
        if (responseLookup.TryGetValue(response.lineId, out DialogueLine responseLine))
        {
            // Enqueue the response lines to continue the dialogue
            lines.Clear(); // Clear any existing lines
            foreach (string line in responseLine.lines)
            {
                // Create a new DialogueLine for each line of the response
                DialogueLine newLine = new DialogueLine
                {
                    lines = new List<string> { line },
                    questions = responseLine.questions
                };
                lines.Enqueue(newLine);
            }

            // Display the first line of the response
            DisplayNextLine();
        }
        else
        {
            Debug.LogError($"Response not found for ID: {response.lineId}");
        }
    }
}