using UnityEngine;

public class Porta : MonoBehaviour
{
    public AudioSource doorSound; // assegna questo da Inspector
    private bool isOpen = false;

    void Update()
    {
        // Il tuo codice che controlla se il player si avvicina
        if (!isOpen && PlayerVicino())
        {
            ApriPorta();
        }
    }

    void ApriPorta()
    {
        isOpen = true;

        // Avvia movimento/animazione qui
        // ...

        // Riproduci suono una sola volta
        if (doorSound != null && !doorSound.isPlaying)
        {
            doorSound.Play();
        }
    }

    bool PlayerVicino()
    {
        // Sostituisci con la tua logica di distanza, trigger, ecc.
        return true;
    }
}

