using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ProjectWork
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        private string volumeParameterName;
        private Slider volumeSlider;

        private void Awake()
        {
            // Use the GameObject's tag as the parameter name
            volumeParameterName = gameObject.tag;

            volumeSlider = GetComponentInChildren<Slider>();
            
            SceneManager.sceneLoaded += HandleSceneLoad;
            volumeSlider.onValueChanged.AddListener(OnChangeSlider);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoad;
            volumeSlider.onValueChanged.RemoveListener(OnChangeSlider);
        }

        private void HandleSceneLoad(Scene scene, LoadSceneMode mode)
        {
            LoadVolumeSetting();
        }

        private void OnChangeSlider(float value)
        {
            if (audioMixer.SetFloat(volumeParameterName, Mathf.Log10(value) * 20f))
            {
                PlayerPrefs.SetFloat(volumeParameterName, value);
            }
            else
            {
                Debug.LogError($"Failed to set audio mixer parameter: {volumeParameterName}");
            }
        }

        private void LoadVolumeSetting()
        {
            float sliderValue = 1f; // Default volume level (1.0)

            if (PlayerPrefs.HasKey(volumeParameterName))
            {
                sliderValue = PlayerPrefs.GetFloat(volumeParameterName);
            }
            else
            {
                PlayerPrefs.SetFloat(volumeParameterName, sliderValue);
            }

            // Set the mixer value
            if(!audioMixer.SetFloat(volumeParameterName, Mathf.Log10(sliderValue) * 20f))
            {
                Debug.LogError($"Failed to set initial audio mixer parameter: {volumeParameterName}");
                return;
            }

            // If you want to get the current mixer value and reflect it on the slider:
            if (audioMixer.GetFloat(volumeParameterName, out float dBValue))
            {
                // Convert dB back to linear (slider) value
                sliderValue = Mathf.Pow(10f, dBValue / 20f);
            }
            else
            {
                Debug.LogError($"Failed to get audio mixer parameter: {volumeParameterName}");
            }

                // Set the slider value (this will not trigger OnChangeSlider if you use .SetValueWithoutNotify)
                volumeSlider.SetValueWithoutNotify(sliderValue);
        }
    }
}