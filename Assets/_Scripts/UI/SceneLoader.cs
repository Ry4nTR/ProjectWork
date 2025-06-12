using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ProjectWork.UI;

namespace ProjectWork
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private UI_Panel screenCaller;
        [SerializeField] private UI_Panel loadingScreen;
        private Slider loadingBarFill;

        private void Awake()
        {
            loadingBarFill = loadingScreen.GetComponentInChildren<Slider>();
        }

        public void LoadScene(string sceneName)
        {
            screenCaller.SetCanvasGroup(false);
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            loadingScreen.SetCanvasGroup(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingBarFill.value = progress;
                yield return null;
            }
        }
    }
}