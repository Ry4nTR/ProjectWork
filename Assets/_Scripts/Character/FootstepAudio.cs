using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepAudio : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.4f;

    private float stepTimer;
    private CharacterController controller;
    private bool wasMovingLastFrame = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stepTimer = 0f; // così parte subito
    }

    void Update()
    {
        bool isMovingNow = IsGroundedAndMoving();

        if (isMovingNow)
        {
            // Se hai appena iniziato a camminare → suono immediato
            if (!wasMovingLastFrame)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        wasMovingLastFrame = isMovingNow;
    }

    bool IsGroundedAndMoving()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return controller.isGrounded && (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0 || footstepSource == null) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.PlayOneShot(clip);
    }
}
