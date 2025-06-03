using ProjectWork;
using UnityEngine;

public class Shower : InteractableObject
{
    [SerializeField] private AudioClip showerSound; // Sound to play when interacting
    [SerializeField] private AudioSource audioSource; // AudioSource component reference

    protected override void InteractChild()
    {
        // Play the shower sound if available
        if (showerSound != null && audioSource != null)
        {
            audioSource.clip = showerSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Shower sound or AudioSource not set up properly");
        }
    }

    protected override void Start()
    {
        base.Start();

        // Subscribe to the black screen finished event if using black screen
        if (isUsingBlackScreen)
        {
            BlackScreenTextController.OnBlackScreenTextFinished += HandleBlackScreenFinished;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when destroyed
        if (isUsingBlackScreen)
        {
            BlackScreenTextController.OnBlackScreenTextFinished -= HandleBlackScreenFinished;
        }
    }

    private void HandleBlackScreenFinished()
    {
        // Stop the sound when black screen finishes
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}