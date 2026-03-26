using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    // This is the special version for Character Controllers
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if the thing we bumped into has the tag
        if (hit.gameObject.CompareTag("Collectible"))
        {
            // Add to the inventory
            InventoryManager.instance.AddFlower(1);

            // Destroy the flower
            Destroy(hit.gameObject);

            Debug.Log("Flower Collected via Character Controller!");
        }
    }
}