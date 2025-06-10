using UnityEngine;
using System.Collections;

public class AudioZoneSwitcher : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audioSource;

    [Header("Settings")]
    [Range(0.1f, 3f)] public float fadeTime = 1.0f;
    [Range(0f, 1f)] public float maxVolume = 1f;

    private Coroutine currentFade;
   

 

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Interrompi qualsiasi fade in corso
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }

        // Avvia la nuova transizione
        currentFade = StartCoroutine(TransitionAudio(true));
        
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Interrompi qualsiasi fade in corso
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }
        currentFade = StartCoroutine(TransitionAudio(false));
    }
    IEnumerator TransitionAudio(bool fadeIn)
    {
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeTime);
            if (fadeIn)
            {
                audioSource.volume = Mathf.Lerp(0f, maxVolume, progress);
            }else             {
                audioSource.volume = Mathf.Lerp(maxVolume, 0f, progress);
            }
            yield return null;
        }

        // Garantisce i valori finali
        audioSource.volume = fadeIn ? maxVolume : 0f;

    }
}