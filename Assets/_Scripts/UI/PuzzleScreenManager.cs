using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    [System.Serializable]
    public class PuzzleScreen
    {
        public GameObject screenParent; // The canvas parent object
        public Image background;       // Background image component
        public TextMeshProUGUI displayText;       // Text component
    }

    public class PuzzleScreenManager : MonoBehaviour
    {
        [Header("Screen Configuration")]
        public PuzzleScreen[] screens; // Array of all screens to manage

        [Header("Active State Colors")]
        public Color activeBackgroundColor = Color.red;
        public Color activeTextColor = Color.white;
        public string activeText = "DESTROY ASTEROIDS!";

        [Header("Completed State Colors")]
        public Color completedBackgroundColor = Color.green;
        public Color completedTextColor = Color.black;
        public string completedText = "COMPLETED!";

        private void OnEnable()
        {
            ProgressBar.OnPuzzleCompleted += HandlePuzzleCompleted;
        }

        private void OnDisable()
        {
            ProgressBar.OnPuzzleCompleted -= HandlePuzzleCompleted;
        }

        public void InitializeScreens()
        {
            foreach (PuzzleScreen screen in screens)
            {
                if (screen.screenParent != null)
                {
                    SetScreenState(screen, false);
                    screen.screenParent.SetActive(true);
                }
            }
        }

        private void HandlePuzzleCompleted()
        {
            foreach (PuzzleScreen screen in screens)
            {
                SetScreenState(screen, true);
            }
        }

        private void SetScreenState(PuzzleScreen screen, bool isCompleted)
        {
            if (screen.background != null)
            {
                screen.background.color = isCompleted ? completedBackgroundColor : activeBackgroundColor;
                screen.displayText.text = isCompleted ? completedText : activeText;
            }

            if (screen.displayText != null)
            {
                screen.displayText.color = isCompleted ? completedTextColor : activeTextColor;
                screen.displayText.text = isCompleted ? completedText : activeText;
            }
        }
    }
}