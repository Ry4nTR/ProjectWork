using UnityEngine;

namespace ProjectWork
{ 
    public class PauseHandler : MonoBehaviour
    {
        public static bool IsPaused => Time.timeScale == 0f;

        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject gameUI;

        private void Awake()
        {
            // Ensure the pause menu is hidden at the start
            ResumeGame();
        }

        private void Update()
        {
            // Check for the pause key (Escape key by default)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        private void TogglePause()
        {
            // Toggle the pause state
            if (Time.timeScale == 0f)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            // Set the time scale to 0 to pause the game
            Time.timeScale = 0f;

            pauseMenu.SetActive(true);
            gameUI.SetActive(false);
            Cursor.visible = true; // Show the cursor when paused
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        }

        public void ResumeGame()
        {
            // Set the time scale back to 1 to resume the game
            Time.timeScale = 1f;

            pauseMenu.SetActive(false);
            gameUI.SetActive(true);
            Cursor.visible = false; // Hide the cursor when resuming
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor back to the center
        }
    }
}
