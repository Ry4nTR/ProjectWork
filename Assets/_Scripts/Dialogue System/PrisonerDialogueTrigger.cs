namespace ProjectWork
{
    public class HangarPrisonerDialogueTrigger : DialogueTrigger
    {
        public override void TriggerDialogue()
        {
            DialogueManager.Instance.StartDialogue(dialogues[TutorialTaskChecker.CurrentDay - 1], dialogueText, questionBoxContainer); // Start the dialogues
        }
    }
}