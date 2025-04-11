using System.Collections;
using ITSProjectWork;
using UnityEngine;

public class Letto : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        BlackScreenTextController.Instance.ActivateBlackScreen("DORMI");
    }


}

