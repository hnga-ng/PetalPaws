using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WorldSpacePrompt : MonoBehaviour
{
    private TextMeshProUGUI hudText;
    public float detectRange = 4.0f;
    public float lookAngleThreshold = 0.8f; // Higher means you have to look more directly at it (0.9 is strict)
    public GameObject particleEffect;

    private Transform playerTransform;
    private MeshRenderer flowerRenderer;
    private Color originalColor;
    private Vector3 originalScale;
    private bool isBeingLookedAt = false;

    void Start()
    {
        playerTransform = Camera.main.transform;
        GameObject textObj = GameObject.Find("InteractionText");
        if (textObj != null) hudText = textObj.GetComponent<TextMeshProUGUI>();

        flowerRenderer = GetComponentInChildren<MeshRenderer>();
        if (flowerRenderer != null) originalColor = flowerRenderer.material.color;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. Check Distance
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 2. Check "Looking At" Direction
        Vector3 dirToFlower = (transform.position - playerTransform.position).normalized;
        float dot = Vector3.Dot(playerTransform.forward, dirToFlower);

        // Only "Select" this flower if it is close AND in front of the camera
        if (distance <= detectRange && dot > lookAngleThreshold)
        {
            if (!isBeingLookedAt)
            {
                isBeingLookedAt = true;
                if (hudText != null) hudText.text = "Press [E] to pick up";
            }

            // Pulse Effect
            float pulse = (Mathf.Sin(Time.time * 5f) + 1f) / 2f;
            if (flowerRenderer != null)
                flowerRenderer.material.color = Color.Lerp(originalColor, Color.white * 2.0f, pulse);
            transform.localScale = originalScale * (1f + (pulse * 0.1f));

            // Pick Up
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                CollectThisFlower();
            }
        }
        else
        {
            // If we walk away OR look away, reset the flower
            if (isBeingLookedAt)
            {
                isBeingLookedAt = false;
                if (hudText != null) hudText.text = "";

                if (flowerRenderer != null) flowerRenderer.material.color = originalColor;
                transform.localScale = originalScale;
            }
        }
    }

    void CollectThisFlower()
    {
        if (hudText != null) hudText.text = "";
        if (particleEffect != null) Instantiate(particleEffect, transform.position, Quaternion.identity);
        InventoryManager.instance.AddFlower(1);
        Destroy(gameObject);
    }
}