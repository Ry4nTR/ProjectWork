using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class FoodSpawner : MonoBehaviour
    {
        [Serializable]
        public class FoodPrefab
        {
            public FoodType foodType;
            public GameObject prefab;
        }

        [SerializeField] private List<FoodPrefab> foodPrefabs;
        [SerializeField] private Transform spawnPoint;

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
            FoodPrefab foodPrefab = foodPrefabs.Find(x => x.foodType == foodType);
            if (foodPrefab != null && foodPrefab.prefab != null)
            {
                GameObject foodObj =  Instantiate(foodPrefab.prefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}