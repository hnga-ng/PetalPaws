using UnityEngine;
using UnityEngine.AI; // Required for NavMesh movement

public class CatCompanion3rd : MonoBehaviour
{
    [Header("Follow Settings")]
    // Drag your 'CatFollowTarget' (the empty object we made) here
    public Transform followTarget;

    [Header("Speed Matching")]
    // The cat will be 15% faster than the girl so it can catch up if it falls behind
    public float catSpeedMultiplier = 1.15f;
    // The slowest the cat will move while heading to its target
    public float minimumWalkSpeed = 2.5f;

    private NavMeshAgent agent;
    private Animator anim;
    private CharacterController girlController; // Reference to the girl's controller

    void Start()
    {
        // Get the components from the cat object
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Find the CharacterController on the girl (the parent of the target)
        if (followTarget != null)
        {
            girlController = followTarget.GetComponentInParent<CharacterController>();
        }

        // Validation check: Warns you if you forgot to drag the target in
        if (followTarget == null)
        {
            Debug.LogWarning("Cat has no followTarget! Drag the CatFollowTarget object into the inspector.");
        }
    }

    void Update()
    {
        if (followTarget == null) return;

        // --- 1. SPEED MATCHING LOGIC ---

        if (girlController != null)
        {
            // We get the girl's actual current speed from her CharacterController
            float girlCurrentSpeed = girlController.velocity.magnitude;

            // We set the cat's speed to match the girl, multiplied by our 'catch up' value
            // Mathf.Max ensures the cat never moves slower than 2.5 if it has a destination
            agent.speed = Mathf.Max(minimumWalkSpeed, girlCurrentSpeed * catSpeedMultiplier);
        }

        // --- 2. MOVEMENT LOGIC ---

        // The cat now simply heads towards the target object we placed near the player's feet
        Vector3 targetPos = followTarget.position;

        // "SamplePosition" ensures the cat stays on the NavMesh floors
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // --- 3. ANIMATION LOGIC ---

        // This sends the cat's actual walking speed into your Animator "Speed" parameter
        if (anim != null)
        {
            // .magnitude turns the velocity (direction) into a single speed number
            float currentSpeed = agent.velocity.magnitude;
            anim.SetFloat("Speed", currentSpeed);
        }

        // --- 4. ROTATION LOGIC ---
        // The NavMeshAgent will now naturally face the direction it is walking.
    }
}