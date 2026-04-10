using UnityEngine;
using UnityEngine.AI; // Required for NavMesh movement

public class CatCompanion : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform playerCamera;  // Drag your Main Camera here
    public float leadDistance = 3.5f; // How far in front of you they stay
    public float sideOffset = 2.0f;   // Positive for Right Cat, Negative for Left Cat

    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        // Get the components from the cat object
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // If you forgot to drag the camera in, this finds the Main Camera automatically
        if (playerCamera == null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // --- 1. MOVEMENT LOGIC ---

        // Calculate the 'Lead Point' in front of the player's view
        Vector3 targetPos = playerCamera.position + (playerCamera.forward * leadDistance);

        // Shift the point to the side so cats don't block your vision
        targetPos += playerCamera.right * sideOffset;

        // "SamplePosition" snaps the target point to the blue NavMesh (Terrain)
        // This stops cats from trying to fly or walk through hills
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 4.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // --- 2. ANIMATION LOGIC ---

        // This sends the cat's actual walking speed into your Animator "Speed" parameter
        if (anim != null)
        {
            // .magnitude turns the velocity (direction) into a single speed number
            float currentSpeed = agent.velocity.magnitude;
            anim.SetFloat("Speed", currentSpeed);
        }

        // --- 3. ROTATION LOGIC ---

        // If the cat is basically standing still, make it look at the player
        if (agent.velocity.magnitude < 0.1f)
        {
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        // Calculate the direction to the player
        Vector3 direction = (playerCamera.position - transform.position).normalized;

        // We set Y to 0 so the cat doesn't tilt its whole body up/down if you jump
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            // Create the rotation and smoothly rotate towards it (Slerp)
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}