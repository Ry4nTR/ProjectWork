using System.Collections;
using ITSProjectWork;
using UnityEngine;

public class Doccia : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        BlackScreenTextController.Instance.ActivateBlackScreen("DOCCIATI");
    }


}

