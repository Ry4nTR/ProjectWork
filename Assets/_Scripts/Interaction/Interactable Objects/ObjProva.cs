using System.Collections;
using UnityEngine;

public class ObjProva : MonoBehaviour, IInteractable
{
    public float pickupDelay = 0.2f; // Small delay before disappearing

    public void Interact()
    {
        StartCoroutine(DisableKey());
    }

    private IEnumerator DisableKey()
    {
        yield return new WaitForSeconds(pickupDelay);
        gameObject.SetActive(false); // Makes the obj disappear
    }
}