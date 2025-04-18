using System;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance { get; private set; }
    public static event Action<Trash> OnTrashSpawned = delegate { };

    [SerializeField] private Trash trash;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void SpawnTrash()
    {
        if (trash != null)
        {
            trash.transform.SetPositionAndRotation(transform.position, transform.rotation);
            trash.gameObject.SetActive(true);
            OnTrashSpawned?.Invoke(trash);
        }
    }
}