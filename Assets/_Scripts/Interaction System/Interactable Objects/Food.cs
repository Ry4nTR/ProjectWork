using System;
using UnityEngine;

namespace ProjectWork
{
    public class Food : InteractableObject
    {
        public static event Action<Food> OnFoodSpawned = delegate { };

        [Header("Food Settings")]
        [SerializeField] private FoodType foodType;

        public FoodType FoodType => foodType;

        protected override void Start()
        {
            base.Start();
            OnFoodSpawned?.Invoke(this);
        }

        protected override void InteractChild()
        {
            Debug.Log("Interacting with Food: " + gameObject.name);
            TrashManager.Instance.SpawnTrash();  // Spawna nel punto del TrashManager
            BlackScreenTextController.OnBlackScreenFullActivated += DestroyObject; // Si iscrive all'evento per distruggere l'oggetto
        }

        private void DestroyObject()
        {
            Debug.Log("Destroying Food Object: " + gameObject.name);
            BlackScreenTextController.OnBlackScreenFullActivated -= DestroyObject; // Si disiscrive dall'evento
            Destroy(gameObject);
        }
    }
}