using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class CollectibleFlower3rd : MonoBehaviour
{
    [Header("Settings")]
    public string itemID = "Morphora";
    public float respawnTime = 20.0f;
    public float detectRange = 3.0f; // Distance from PLAYER body, not camera
    public GameObject particleEffect;

    [Header("3rd Person Pulse")]
    public float pulseSpeed = 4f;   // Faster speed for more "drama"
    public float pulseSize = 0.3f;  // Grows by 30% instead of 10%

    [Header("Audio")]
    public AudioClip collectSound;
    [Range(0, 1)] public float volume = 1.0f;

    [Header("References")]
    private TextMeshProUGUI hudText;
    private Transform playerBody; // Target the character, not the camera lens
    private MeshRenderer myRenderer;
    private BoxCollider myCollider;

    private bool IAmShowingText = false;
    private bool isCollected = false;
    private Vector3 originalScale;
    private AudioSource myAudioSource;

    void Start()
    {
        // FIND THE PLAYER BODY: 
        // We look for the tag "Player". Make sure your 3rd Person Controller is tagged "Player"!
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerBody = playerObj.transform;

        hudText = GameObject.Find("InteractionText")?.GetComponent<TextMeshProUGUI>();
        myRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<BoxCollider>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isCollected || playerBody == null) return;

        // Calculate distance between the flower and the Player's body
        float distance = Vector3.Distance(transform.position, playerBody.position);

        // LOGIC: Just check distance. No "LookAngle" needed for 3rd person proximity.
        if (distance <= detectRange)
        {
            if (hudText != null && !IAmShowingText)
            {
                hudText.text = "Press [E] to pick up " + itemID;
                IAmShowingText = true;
            }

            // --- DRAMATIC PULSE ---
            // Increased the math here so it's very obvious from a distance
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            transform.localScale = originalScale * (1f + (pulse * pulseSize));

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(CollectAndRespawn());
            }
        }
        else
        {
            if (IAmShowingText)
            {
                if (hudText != null) hudText.text = "";
                IAmShowingText = false;
                transform.localScale = originalScale;
            }
        }
    }

    IEnumerator CollectAndRespawn()
    {
        isCollected = true;

        if (collectSound != null)
        {
            if (myAudioSource == null) myAudioSource = GetComponent<AudioSource>();
            if (myAudioSource != null) myAudioSource.PlayOneShot(collectSound, volume);
        }

        if (IAmShowingText && hudText != null) hudText.text = "";
        IAmShowingText = false;
        transform.localScale = originalScale;

        if (particleEffect != null) Instantiate(particleEffect, transform.position, Quaternion.identity);

        // Ensure you have your InventoryManager script in the scene!
        if (InventoryManager.instance != null) InventoryManager.instance.AddItem(itemID, 1);

        if (myRenderer != null) myRenderer.enabled = false;
        if (myCollider != null) myCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        if (myRenderer != null) myRenderer.enabled = true;
        if (myCollider != null) myCollider.enabled = true;
        isCollected = false;
    }
}