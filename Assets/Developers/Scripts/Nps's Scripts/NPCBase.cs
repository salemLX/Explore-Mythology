using UnityEngine;
using UnityEngine.AI;


// Author: Salem
// Created: [24-11-2026]


// Base class for all NPCs in the game
[RequireComponent(typeof(NavMeshAgent))]
public abstract class NPCBase : MonoBehaviour, IPanicable
{
    [Header("NPC Settings")]
    [SerializeField] protected float moveSpeed = 1.5f;

    [Header("Panic Settings")]
    [SerializeField] private float panicSpeed = 4f;
    [SerializeField] private float panicDuration = 5f;
    [SerializeField] private float fleeDistance = 8f;
    [SerializeField] private float fleeUpdateInterval = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Panic Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] panicSounds;
    [SerializeField] private Vector2 soundInterval = new(2f, 5f);

    [Header("Panic Visual")]
    [SerializeField] private GameObject panicIndicator;

    protected NavMeshAgent agent;
    protected Vector3 spawnPosition;

    protected bool isPanicking;

    private Vector3 dangerPosition;
    private float panicEndTime;
    private float nextFleeTime;
    private float nextSoundTime;

    private static readonly float[] fleeAngles =
    {
        0f, 35f, -35f, 70f, -70f, 110f, -110f, 180f
    };



    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (panicIndicator != null)
            panicIndicator.SetActive(false);
    }

    protected virtual void Start()
    {
        spawnPosition = transform.position;
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (!agent.isOnNavMesh)
            return;

        if (isPanicking)
            UpdatePanic();
        else
            UpdateBehaviour();

        if (animator != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    protected abstract void UpdateBehaviour();

    protected virtual void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    protected virtual void StopMoving()
    {
        agent.ResetPath();
    }

    // Called through IPanicable.
    public virtual void Panic(Vector3 newDangerPosition)
    {
        if (!agent.isOnNavMesh)
            return;

        dangerPosition = newDangerPosition;
        panicEndTime = Time.time + panicDuration;
        nextFleeTime = 0f;

        if (isPanicking)
            return;

        isPanicking = true;
        agent.speed = panicSpeed;

        if (panicIndicator != null)
            panicIndicator.SetActive(true);

        StopMoving();

        if (animator != null)
            animator.SetBool("IsPanicking", true);

        nextSoundTime = Time.time +
            Random.Range(0.2f, 1f);

        OnPanicStarted();
    }

    private void UpdatePanic()
    {
        if (Time.time >= panicEndTime)
        {
            StopPanic();
            return;
        }

        if (Time.time >= nextFleeTime)
        {
            FleeFromDanger();
            nextFleeTime = Time.time +
                fleeUpdateInterval;
        }

        PlayRandomPanicSound();
    }



    private void FleeFromDanger()
    {
        // Try to find a random escape position.
        for (int i = 0; i < 15; i++)
        {
            Vector2 random = Random.insideUnitCircle
                             * fleeDistance;

            Vector3 target = transform.position +
                new Vector3(random.x, 0f, random.y);

            if (!NavMesh.SamplePosition(
                target,
                out NavMeshHit hit,
                2f,
                agent.areaMask))
                continue;

            // Avoid destinations too close to the NPC.
            if (Vector3.Distance(
                transform.position, hit.position) < 1.5f)
                continue;

            NavMeshPath path = new NavMeshPath();

            if (!agent.CalculatePath(hit.position, path))
                continue;

            if (path.status != NavMeshPathStatus.PathComplete)
                continue;

            // Run towards the random destination.
            agent.SetPath(path);
            return;
        }

        Debug.LogWarning($"{name}: No panic destination found!");
    }



    private void PlayRandomPanicSound()
    {
        if (Time.time < nextSoundTime)
            return;

        if (audioSource != null && panicSounds.Length > 0)
        {
            AudioClip clip = panicSounds[
                Random.Range(0, panicSounds.Length)
            ];

            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        nextSoundTime = Time.time +
            Random.Range(soundInterval.x, soundInterval.y);
    }

    private void StopPanic()
    {
        isPanicking = false;
        agent.speed = moveSpeed;

        StopMoving();

        if (animator != null)
            animator.SetBool("IsPanicking", false);

        if (panicIndicator != null)
            panicIndicator.SetActive(false);

        OnPanicEnded();
    }

    protected virtual void OnPanicStarted() { }

    protected virtual void OnPanicEnded() { }
}
