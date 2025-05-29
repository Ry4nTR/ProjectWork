namespace ProjectWork
{
    public class PrisonerDialogueTrigger : DialogueTrigger
    {
        public override void TriggerDialogue()
        {
            DialogueManager.Instance.StartDialogue(dialogues[TutorialTaskChecker.CurrentDay - 1], dialogueText, questionBoxContainer); // Start the dialogues
        }
    }
}