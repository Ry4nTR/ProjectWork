// AIDialogueManager.cs
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using NavKeypad;

namespace ProjectWork
{
    public class AIDialogueManager : MonoBehaviour
    {
        public static event Action<string, bool> OnNewMessage = delegate { }; // testo + isAISpeaking
        public static event Action OnDialogueStart = delegate { };
        public static event Action OnDialogueEnd = delegate { };

        [Range(0.01f, 0.5f)]
        public float typingSpeed = 0.05f;

        private Coroutine currentDialogueRoutine;
        private Coroutine typingRoutine;
        private bool isDialogueActive;

        [Header("Impostazioni Chiusura")]
        [SerializeField] private float endDelay = 2f; // Secondi prima della chiusura

        [Header("Test")]
        [SerializeField] private AIDialogue[] dialogues;

        private void Awake()
        {
            BlackScreenTextController.OnInitialBlackScreenFinished += StartFirstDialogue;
            TutorialTaskChecker.OnDayPassed += StartSecondDialogue;
            Keypad.OnKeypadUnlocked += StartThirdDialogue;
            PuzzleChecker.OnAllPuzzlesCompleted += StartFourthDialogue;
        }

        private void OnDestroy()
        {
            BlackScreenTextController.OnInitialBlackScreenFinished -= StartFirstDialogue;
            TutorialTaskChecker.OnDayPassed -= StartSecondDialogue;
            Keypad.OnKeypadUnlocked -= StartThirdDialogue;
            PuzzleChecker.OnAllPuzzlesCompleted -= StartFourthDialogue;
        }


        private void StartFourthDialogue()
        {
            FindAndStartDialogueWithName("DIAL06");
        }

        private void StartThirdDialogue()
        {
            FindAndStartDialogueWithName("DIAL04");
        }

        private void StartSecondDialogue(bool areDaysPassed)
        {
            if (areDaysPassed)
            {
                FindAndStartDialogueWithName("DIAL03");
            }
        }

        private void StartFirstDialogue()
        {
            FindAndStartDialogueWithName("DIAL01");
        }

        private void FindAndStartDialogueWithName(string dialogueName)
        {
            AIDialogue dialogue = dialogues.FirstOrDefault(d => d.name == dialogueName);
            if (dialogue != null)
            {
                StartDialogue(dialogue);
            }
            else
            {
                Debug.LogWarning($"Dialogue '{dialogueName}' not found.");
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
}