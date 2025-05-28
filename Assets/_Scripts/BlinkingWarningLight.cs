using UnityEngine;

namespace ProjectWork
{
    public class BlinkingWarningLight : MonoBehaviour
    {
        [SerializeField] private MeshRenderer targetMeshRenderer;

        private static readonly int IsBlinkingID = Shader.PropertyToID("_isBlinking");

        private void Awake()
        {
            TutorialTaskChecker.OnDayPassed += TrySetBlink;
        }

        private void Start()
        {
            SetBlink(false);
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= TrySetBlink;
        }

        private void TrySetBlink(bool areDaysOver)
        {
            SetBlink(areDaysOver);
        }

        private void SetBlink(bool isBlinking)
        {
            targetMeshRenderer.material.SetFloat(IsBlinkingID, isBlinking ? 1f : 0f);
        }
    }
}
