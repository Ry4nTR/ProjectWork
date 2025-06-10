using UnityEngine;
using System.Collections;

public class AudioZoneSwitcher : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audioCella;
    public AudioSource audioSala;

    [Header("Settings")]
    [Range(0.1f, 3f)] public float fadeTime = 1.0f;
    [Range(0f, 1f)] public float maxVolume = 1f;

    private Coroutine currentFade;
    private bool inSala = false;

    void Start()
    {
        // Configurazione iniziale garantita
        audioCella.volume = maxVolume;
        audioSala.volume = 0;
        audioCella.Play();
        audioSala.Play(); // Entrambi sempre attivi ma con volume controllato
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Determina la direzione della transizione
        bool enteringSala = gameObject.name.Contains("Sala"); // Adatta al naming dei tuoi trigger

        // Evita transizioni ridondanti
        if (enteringSala == inSala) return;

        inSala = enteringSala;

        // Interrompi qualsiasi fade in corso
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }

        // Avvia la nuova transizione
        currentFade = StartCoroutine(TransitionAudio(
            enteringSala ? audioCella : audioSala,
            enteringSala ? audioSala : audioCella
        ));
    }

    IEnumerator TransitionAudio(AudioSource fadeOut, AudioSource fadeIn)
    {
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeTime);

            fadeOut.volume = Mathf.Lerp(maxVolume, 0f, progress);
            fadeIn.volume = Mathf.Lerp(0f, maxVolume, progress);

            yield return null;
        }

        // Garantisce i valori finali
        fadeOut.volume = 0;
        fadeIn.volume = maxVolume;
    }
}