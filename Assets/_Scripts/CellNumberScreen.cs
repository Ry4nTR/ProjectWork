using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectWork
{
    public class CellNumberScreen : MonoBehaviour
    {
        [Serializable]
        private struct Colors
        {
            public Color normalColor;
            public Color emergencyColor;
        }

        [SerializeField] private Colors backgroundColors;
        [SerializeField] private Colors textColors;

        [Header("References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI cellNumberText;

        private void Awake()
        {
            TutorialTaskChecker.OnDayPassed += SetScreenColors;
        }

        private void Start()
        {
            SetScreenColors(false); // Set initial colors based on normal state
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= SetScreenColors;
        }

        private void SetScreenColors(bool isEmergency)
        {
            backgroundImage.color = isEmergency ? backgroundColors.emergencyColor : backgroundColors.normalColor;
            cellNumberText.color = isEmergency ? textColors.emergencyColor : textColors.normalColor;
        }
    }
}