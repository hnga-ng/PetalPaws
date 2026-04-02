using UnityEngine;
using TMPro; // Make sure you are using TextMeshPro for the UI text
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance; // Allows other scripts to find this easily

    [Header("Morphora Settings")]
    public int morphoraCount = 0;
    public TextMeshProUGUI morphoraText; // Drag Morphora UI here

    [Header("Pinkling Settings")]
    public int pinklingCount = 0;
    public TextMeshProUGUI pinklingText; // Drag Pinkling UI here

    [Header("Tinker Settings")]
    public int tinkerCount = 0;
    public TextMeshProUGUI tinkerText; // Drag TInker UI here

    void Awake()
    {
        // Singleton pattern: makes sure there is only one manager
        instance = this;
    }

    void Start()
    {
        // Set the text to 0 at the start so it's not empty
        UpdateUI();
    }

    public void AddItem(string itemName, int amount)
    {
        if (itemName == "Morphora")
        {
            morphoraCount += amount;
        }
        else if (itemName == "Pinkling")
        {
            pinklingCount += amount;
        }
        else if (itemName == "Tinker")
        {
            tinkerCount += amount;
        }

        UpdateUI(); // Refresh the numbers on screen
    }

    // This helper function keeps the screen updated
    void UpdateUI()
    {
        if (morphoraText != null)
            morphoraText.text = morphoraCount.ToString();

        if (pinklingText != null)
            pinklingText.text = pinklingCount.ToString();

        if (tinkerText != null)
            tinkerText.text = tinkerCount.ToString();
    }
}