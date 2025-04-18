using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProjectWork.UI
{
    public class PadScreen : UI_Panel
    {
        [SerializeField] private FoodPad.PadState _screenType;
        private List<BoxCollider> buttonColliders;

        public FoodPad.PadState ScreenType => _screenType;

        protected override void Awake()
        {
            base.Awake();
            buttonColliders = GetComponentsInChildren<BoxCollider>(true).ToList();
        }

        public void SetScreenVisibility(bool isVisible)
        {
            SetCanvasGroup(isVisible);
            foreach (BoxCollider collider in buttonColliders)
            {
                collider.enabled = isVisible;
            }
        }

        public void SetScreenType(FoodPad.PadState screenType)
        {
            _screenType = screenType;
        }
    }
}