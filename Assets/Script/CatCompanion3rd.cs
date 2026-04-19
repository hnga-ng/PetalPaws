using UnityEngine;
using UnityEngine.AI; // Required for NavMesh movement

public class CatCompanion3rd : MonoBehaviour
{
    [Header("Follow Settings")]
    // Drag your 'CatFollowTarget' (the empty object we made) here
    public Transform followTarget;

    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        // Get the components from the cat object
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Validation check: Warns you if you forgot to drag the target in
        if (followTarget == null)
        {
            Debug.LogWarning("Cat has no followTarget! Drag the CatFollowTarget object into the inspector.");
        }
    }

    void Update()
    {
        if (followTarget == null) return;

        // --- 1. MOVEMENT LOGIC ---

        // The cat now simply heads towards the target object we placed near the player's feet
        Vector3 targetPos = followTarget.position;

        // "SamplePosition" ensures the cat stays on the NavMesh floors
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 2.0f, NavMesh.AllAreas))
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
        // 'LookAtMe' function has been removed. 
        // The NavMeshAgent will now naturally face the direction it is walking.
    }
}