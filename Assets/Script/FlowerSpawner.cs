using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class CollectibleFlower : MonoBehaviour
{
    [Header("Settings")]
    public float respawnTime = 10.0f;
    public float detectRange = 5.0f;
    public float lookAngleThreshold = 0.5f;
    public GameObject particleEffect;

    [Header("References")]
    private TextMeshProUGUI hudText;
    private Transform playerTransform;
    private MeshRenderer myRenderer;
    private BoxCollider myCollider;

    private bool IAmShowingText = false;
    private bool isCollected = false;

    // --- PULSE VARIABLES ---
    private Vector3 originalScale;
    private Color originalColor;

    void Start()
    {
        playerTransform = Camera.main.transform;
        hudText = GameObject.Find("InteractionText")?.GetComponent<TextMeshProUGUI>();
        myRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<BoxCollider>();

        // Store original looks for the pulse
        originalScale = transform.localScale;
        if (myRenderer != null) originalColor = myRenderer.material.color;
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
                hudText.text = "Press [E] to pick up";
                IAmShowingText = true;
            }

            // --- THE PULSE ANIMATION IS BACK ---
            float pulse = (Mathf.Sin(Time.time * 5f) + 1f) / 2f;
            if (myRenderer != null)
                myRenderer.material.color = Color.Lerp(originalColor, Color.white * 1.5f, pulse);
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
                if (myRenderer != null) myRenderer.material.color = originalColor;
                transform.localScale = originalScale;
            }
        }
    }

    IEnumerator CollectAndRespawn()
    {
        isCollected = true;

        if (IAmShowingText && hudText != null) hudText.text = "";
        IAmShowingText = false;

        // Reset scale/color before hiding so it's fresh when it respawns
        transform.localScale = originalScale;
        if (myRenderer != null) myRenderer.material.color = originalColor;

        if (particleEffect != null) Instantiate(particleEffect, transform.position, Quaternion.identity);
        InventoryManager.instance.AddFlower(1);

        if (myRenderer != null) myRenderer.enabled = false;
        if (myCollider != null) myCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        if (myRenderer != null) myRenderer.enabled = true;
        if (myCollider != null) myCollider.enabled = true;
        isCollected = false;
    }
}