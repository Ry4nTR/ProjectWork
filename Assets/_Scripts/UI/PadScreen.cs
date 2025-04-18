using UnityEngine;

namespace ProjectWork.UI
{
    public class PadScreen : UI_Panel
    {
        [SerializeField] private FoodPad.PadState _screenType;

        public FoodPad.PadState ScreenType => _screenType;
    }
}