using UnityEngine;

public class AudioZoneManager : MonoBehaviour
{
    public AudioSource cellaAudioSource;
    public AudioSource salaAudioSource;
    public float fadeTime = 0.5f; // Tempo per il fade (puoi impostarlo a 0 per un cut secco)

    // Metodo pubblico per cambiare audio
    public void SwitchToSalaAudio()
    {
        StartCoroutine(FadeAudio(cellaAudioSource, salaAudioSource));
    }

    // Metodo pubblico per tornare all'audio della cella (se necessario)
    public void SwitchToCellaAudio()
    {
        StartCoroutine(FadeAudio(salaAudioSource, cellaAudioSource));
    }

    private System.Collections.IEnumerator FadeAudio(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        float elapsedTime = 0f;
        float startVolumeOut = fadeOutSource.volume;
        float startVolumeIn = fadeInSource.volume;

        fadeInSource.Play(); // Avvia il nuovo suono

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeTime;

            fadeOutSource.volume = Mathf.Lerp(startVolumeOut, 0f, t);
            fadeInSource.volume = Mathf.Lerp(0f, startVolumeIn, t);
            yield return null;
        }

        fadeOutSource.Stop();
    }
}