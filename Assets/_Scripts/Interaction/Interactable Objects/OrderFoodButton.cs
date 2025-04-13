using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Needed for pointer interfaces

namespace ProjectWork
{
    public enum FoodType
    {
        Pizza,
        Chicken,
        Donut,
    }

    public class OrderFoodButton : InteractableObject, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] public FoodType foodType;
        [SerializeField] private Color hoverColor = Color.yellow; // Default hover color

        private Color normalColor; // To store the original color
        private Graphic targetGraphic; // The graphic that will change color

        public Button Button => button;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            // Get the target graphic (usually the button's image)
            targetGraphic = button.targetGraphic;
            if (targetGraphic != null)
            {
                normalColor = targetGraphic.color; // Store the original color
            }
        }

        public override void Interact()
        {
            button.onClick.Invoke();
        }

        // Called when mouse enters the button area
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetGraphic != null)
            {
                targetGraphic.color = hoverColor;
            }
        }

        // Called when mouse exits the button area
        public void OnPointerExit(PointerEventData eventData)
        {
            if (targetGraphic != null)
            {
                targetGraphic.color = normalColor;
            }
        }
    }
}