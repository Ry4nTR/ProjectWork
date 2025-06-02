using UnityEngine;

public class Scientist : InteractableObject
{
    private ScientistDialogueTrigger dialogueTrigger;

    protected void Awake()
    {
        dialogueTrigger = GetComponent<ScientistDialogueTrigger>();

        if (dialogueTrigger == null)
        {
            Debug.LogError("Missing ScientistDialogueTrigger component!", this);
        }
    }

    protected override void InteractChild()
    {
        if (dialogueTrigger != null)
        {
            dialogueTrigger.TriggerDialogue();
        }
    }
}