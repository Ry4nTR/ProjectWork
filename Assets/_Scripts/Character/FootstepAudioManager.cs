using UnityEngine;
using System.Collections.Generic;
using System.IO;

[RequireComponent(typeof(CharacterController))]
public class FootstepAudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public float stepInterval = 0.4f;
    public float raycastDistance = 2f;
    public string footstepsBasePath = "CustomAudio/Footsteps";

    private float stepTimer;
    private CharacterController controller;
    private bool wasMovingLastFrame = false;

    private Dictionary<string, List<AudioClip>> surfaceClips = new();
    private List<AudioClip> defaultClips = new();

    private string currentSurface = "";
    private bool stairsSoundPlaying = false;
    private float stairsCooldown = 6f; // tempo tra una riproduzione e l'altra
    private float lastStairsPlayTime = -10f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stepTimer = 0f;
        LoadFootstepClips();
    }

    void Update()
    {
        currentSurface = GetCurrentSurface();
        bool isMoving = IsGroundedAndMoving();

        if (currentSurface == "Stairs")
        {
            HandleStairs(isMoving);
        }
        else
        {
            if (stairsSoundPlaying)
            {
                audioSource.Stop();
                stairsSoundPlaying = false;
            }

            if (isMoving)
            {
                HandleNormalFootsteps();
            }
            else
            {
                stepTimer = 0f;
            }
        }

        wasMovingLastFrame = isMoving;
    }

    void HandleStairs(bool isMoving)
    {
        if (!isMoving)
        {
            // fermo sulle scale = stop audio
            if (stairsSoundPlaying)
            {
                audioSource.Stop();
                stairsSoundPlaying = false;
            }
            return;
        }

        // se si sta muovendo sulle scale e non è già in riproduzione
        if (!stairsSoundPlaying && Time.time - lastStairsPlayTime > stairsCooldown)
        {
            if (surfaceClips.TryGetValue("Stairs", out var clips) && clips.Count > 0)
            {
                AudioClip stairsClip = clips[0]; // primo suono nella cartella Stairs
                audioSource.loop = false;
                audioSource.PlayOneShot(stairsClip);
                stairsSoundPlaying = true;
                lastStairsPlayTime = Time.time;
                Invoke(nameof(ResetStairsFlag), stairsClip.length);
            }
        }
    }

    void ResetStairsFlag()
    {
        stairsSoundPlaying = false;
    }

    void HandleNormalFootsteps()
    {
        stepTimer -= Time.deltaTime;
        if (!wasMovingLastFrame || stepTimer <= 0f)
        {
            PlayFootstep();
            stepTimer = stepInterval;
        }
    }

    void PlayFootstep()
    {
        if (!audioSource || stairsSoundPlaying) return;

        if (surfaceClips.TryGetValue(currentSurface, out var clips) && clips.Count > 0)
        {
            var clip = clips[Random.Range(0, clips.Count)];
            audioSource.PlayOneShot(clip);
        }
        else if (defaultClips.Count > 0)
        {
            var fallback = defaultClips[Random.Range(0, defaultClips.Count)];
            audioSource.PlayOneShot(fallback);
        }
    }

    string GetCurrentSurface()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            return LayerMask.LayerToName(hit.collider.gameObject.layer);
        }

        return "";
    }

    bool IsGroundedAndMoving()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return controller.isGrounded && (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);
    }

    void LoadFootstepClips()
    {
        string fullPath = Path.Combine(Application.dataPath, "Resources", footstepsBasePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError("Cartella footsteps non trovata: " + fullPath);
            return;
        }

        string[] subfolders = Directory.GetDirectories(fullPath);
        foreach (string folder in subfolders)
        {
            string folderName = Path.GetFileName(folder);
            string resourcePath = Path.Combine(footstepsBasePath, folderName).Replace("\\", "/");

            AudioClip[] clips = Resources.LoadAll<AudioClip>(resourcePath);
            if (clips.Length == 0) continue;

            if (folderName.ToLower() == "default")
                defaultClips = new List<AudioClip>(clips);
            else
                surfaceClips[folderName] = new List<AudioClip>(clips);
        }

        if (defaultClips.Count == 0)
        {
            Debug.LogWarning("Nessun fallback audio trovato nella cartella 'Default'.");
        }
    }
}
