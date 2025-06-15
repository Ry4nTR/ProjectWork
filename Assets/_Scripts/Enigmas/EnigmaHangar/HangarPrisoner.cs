using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(Collider))]
    public class HangarPrisoner : InteractableObject
    {
        [SerializeField] private GameObject objectToSpawn; // Assegna l'oggetto da spawnare nell'Inspector
        [SerializeField] private Transform spawnPosition; // Posizione dove spawnare l'oggetto

        private Collider npcCollider;
        private bool hasInteracted = false;

        private void Awake()
        {
            npcCollider = GetComponent<Collider>();
            DialogueManager.OnDialogueStarted += DisableCollider;
            DialogueManager.OnDialogueFinished += OnDialogueFinished;
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueStarted -= DisableCollider;
            DialogueManager.OnDialogueFinished -= OnDialogueFinished;
        }

        private void EnableCollider() => npcCollider.enabled = true;
        private void DisableCollider() => npcCollider.enabled = false;

        protected override void InteractChild()
        {
            if (hasInteracted) return;

            var dialogueTrigger = GetComponent<HangarPrisonerDialogueTrigger>();
            if (dialogueTrigger != null)
            {
                dialogueTrigger.TriggerDialogue();
            }
            else
            {
                Debug.LogError("No HangarPrisonerDialogueTrigger component found!", this);
            }
        }

        private void OnDialogueFinished(InteractableObject character)
        {
            if (hasInteracted) return;
            if(character.GetType() != typeof(HangarPrisoner))
            {
                return;
            }

            hasInteracted = true;
            LockInteraction(); // Disabilita ulteriori interazioni
            DisableCollider(); // Disabilita il collider

            // Spawn dell'oggetto
            if (objectToSpawn != null && spawnPosition != null)
            {
                Instantiate(objectToSpawn, spawnPosition.position, spawnPosition.rotation);
            }
            else
            {
                Debug.LogWarning("Object to spawn or spawn position not assigned!", this);
            }
        }
    }
}