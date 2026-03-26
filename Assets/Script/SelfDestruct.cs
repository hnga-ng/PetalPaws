using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime = 2.0f; // How many seconds until it disappears?

    void Start()
    {
        // This tells Unity: "Wait 2 seconds, then delete this object."
        Destroy(gameObject, lifetime);
    }
}