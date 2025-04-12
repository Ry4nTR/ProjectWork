using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class FoodSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class FoodPrefab
        {
            public FoodType foodType;
            public GameObject prefab;
        }

        [SerializeField] private List<FoodPrefab> foodPrefabs = new List<FoodPrefab>();
        [SerializeField] private Transform spawnPoint; // Where the food will appear
        [SerializeField] private float destroyDelay = 10f; // Time before food is automatically destroyed

        private GameObject currentFoodInstance; // Currently spawned food

        private void OnEnable()
        {
            FoodPad.OnSelectedFood += SpawnFood;
        }

        private void OnDisable()
        {
            FoodPad.OnSelectedFood -= SpawnFood;
        }

        private void SpawnFood(FoodType foodType)
        {
            // Destroy current food if it exists
            if (currentFoodInstance != null)
            {
                Destroy(currentFoodInstance);
            }

            // Find the correct prefab for the ordered food
            FoodPrefab foodPrefab = foodPrefabs.Find(x => x.foodType == foodType);
            if (foodPrefab != null && foodPrefab.prefab != null)
            {
                // Instantiate the food at the spawn point
                currentFoodInstance = Instantiate(foodPrefab.prefab, spawnPoint.position, spawnPoint.rotation);

                // Set up automatic destruction
                Destroy(currentFoodInstance, destroyDelay);

                Debug.Log($"Spawned {foodType} at {spawnPoint.position}");
            }
            else
            {
                Debug.LogWarning($"No prefab assigned for food type: {foodType}");
            }
        }
    }
}