using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoorStatusUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color blockedColor = Color.red;
    [SerializeField] private Color completedColor = Color.green;

    [Header("Messages")]
    [SerializeField] private string normalMessage = "Turn Crank";
    [SerializeField] private string blockedMessage = "UNLOCK THE DOOR";
    [SerializeField] private string completedMessage = "DOOR CLOSED";

    private void Start()
    {
        ResetToNormal();
    }

    public void SetBlockedStatus()
    {
        backgroundImage.color = blockedColor;
        statusText.text = blockedMessage;
    }

    public void SetCompletedStatus()
    {
        backgroundImage.color = completedColor;
        statusText.text = completedMessage;
    }

    public void ResetToNormal()
    {
        backgroundImage.color = normalColor;
        statusText.text = normalMessage;
    }
}