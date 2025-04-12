using UnityEngine;

namespace ProjectWork.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UI_Panel : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;

        [Tooltip("Should set panel visible at start?")]
        [SerializeField] protected bool setInitialVisibility;

        [Tooltip("Should the panel be visible when starting?\n\nNOTE: USED ONLY WHEN \"setInitialVisibility\" = true")]
        [SerializeField] protected bool isVisibleAtStart;       

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if(setInitialVisibility)
            {
                SetCanvasGroup(isVisibleAtStart);
            }
        }

        /// <summary>
        /// Set panel visibility and interactivity
        /// </summary>
        public void SetCanvasGroup(bool isVisible)
        {
            canvasGroup.alpha = isVisible ? 1 : 0;
            canvasGroup.blocksRaycasts = isVisible;
            canvasGroup.interactable = isVisible;
        }
    }
}