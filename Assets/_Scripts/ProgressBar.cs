using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider progressSlider;
    public float successValue = 100f;
    public float failPenalty = 20f; // Penalty per escaped asteroid

    void Start()
    {
        progressSlider.value = 50f; // Start halfway
    }

    public void AddProgress(float amount)
    {
        progressSlider.value += amount;
        if (progressSlider.value >= successValue) PuzzleComplete();
    }

    public void AsteroidEscaped()
    {
        progressSlider.value -= failPenalty;
        if (progressSlider.value <= 0) PuzzleFailed();
    }

    void PuzzleComplete() { /* Handle win */ }
    void PuzzleFailed() { /* Handle loss (reload?) */ }
}