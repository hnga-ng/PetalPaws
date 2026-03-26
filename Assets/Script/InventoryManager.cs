using UnityEngine;
using TMPro; // Make sure you are using TextMeshPro for the UI text
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance; // Allows other scripts to find this easily

    public int flowerCount = 0;
    public TextMeshProUGUI flowerText; // Drag your UI Text here

    void Awake()
    {
        // Singleton pattern: makes sure there is only one manager
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddFlower(int amount)
    {
        flowerCount += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (flowerText != null)
        {
            flowerText.text = flowerCount.ToString();
        }
    }
}