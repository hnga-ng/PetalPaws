using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class CollectibleFlower : MonoBehaviour
{
    [Header("Settings")]
    public string itemID = "Morphora"; // Just for reference, not used in logic here
    public float respawnTime = 20.0f;
    public float detectRange = 5.0f;
    public float lookAngleThreshold = 0.5f;
    public GameObject particleEffect;

    [Header("Audio")] // <--- NEW SECTION - Collect sound settings
    public AudioClip collectSound;
    [Range(0, 1)] public float volume = 1.0f;

    [Header("References")]
    private TextMeshProUGUI hudText;
    private Transform playerTransform;
    private MeshRenderer myRenderer;
    private BoxCollider myCollider;

    private bool IAmShowingText = false;
    private bool isCollected = false;

    // --- PULSE VARIABLES ---
    private Vector3 originalScale;
    private AudioSource myAudioSource; // New reference

    void Start()
    {
        playerTransform = Camera.main.transform;
        // This finds the "Press E" text on screen
        hudText = GameObject.Find("InteractionText")?.GetComponent<TextMeshProUGUI>();
        myRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<BoxCollider>();

        // Store original looks for the pulse
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isCollected || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 dirToFlower = (transform.position - playerTransform.position).normalized;
        float dot = Vector3.Dot(playerTransform.forward, dirToFlower);

        // LOGIC: Am I the "chosen" one?
        if (distance <= detectRange && dot > lookAngleThreshold)
        {
            if (hudText != null && !IAmShowingText)
            {
                hudText.text = "Press [E] to pick up " + itemID;
                IAmShowingText = true;
            }

            // --- THE PULSE ANIMATION ---
            float pulse = (Mathf.Sin(Time.time * 5f) + 1f) / 2f;
            transform.localScale = originalScale * (1f + (pulse * 0.1f));

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(CollectAndRespawn());
            }
        }
        else
        {
            // Reset visuals and text if we look away or walk away
            if (IAmShowingText)
            {
                if (hudText != null) hudText.text = "";
                IAmShowingText = false;
                // RESET VISUALS
                transform.localScale = originalScale;
            }
        }
    }

    IEnumerator CollectAndRespawn()
    {
        isCollected = true;

        // --- PLAY 2D SOUND ---

        if (collectSound != null)
        {
            if (myAudioSource == null) myAudioSource = GetComponent<AudioSource>();

            if (myAudioSource != null)
            {
                myAudioSource.PlayOneShot(collectSound, volume);
            }
            else
            {
                Debug.LogWarning("Collect sound is assigned but no AudioSource found on " + gameObject.name);
            }
        }

        // Clear "Press E" text
        if (IAmShowingText && hudText != null) hudText.text = "";
        IAmShowingText = false;

        // Reset scale/color before hiding so it's fresh when it respawns
        transform.localScale = originalScale;

        // Visuals and inventory
        if (particleEffect != null) Instantiate(particleEffect, transform.position, Quaternion.identity);
        InventoryManager.instance.AddItem(itemID, 1);

        // Hide the flower
        if (myRenderer != null) myRenderer.enabled = false;
        if (myCollider != null) myCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // Bring the flower back
        if (myRenderer != null) myRenderer.enabled = true;
        if (myCollider != null) myCollider.enabled = true;
        isCollected = false;
    }
}