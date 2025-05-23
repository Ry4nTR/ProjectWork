using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepAudio : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;

    private float stepTimer;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsGroundedAndMoving())
        {
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
    }

    bool IsGroundedAndMoving()
    {
        // Movimento base (da tastiera)
        bool moving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        return controller.isGrounded && moving;
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            //Debug.Log("Suono passo: " + clip.name); // Verifica che parta
            footstepSource.PlayOneShot(clip);
        }
    }
}
