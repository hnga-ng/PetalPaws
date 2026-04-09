using UnityEngine;

public class StayAlive : MonoBehaviour
{
    private static StayAlive instance;

    void Awake()
    {
        // This part prevents duplicates if you go back to the menu
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}