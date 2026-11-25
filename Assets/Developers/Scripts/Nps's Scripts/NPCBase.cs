using UnityEngine;
using UnityEngine.AI;


// Author: Salem
// Created: [24-11-2026]


// Base class for all NPCs in the game
[RequireComponent(typeof(NavMeshAgent))]
public abstract class NPCBase : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] protected float moveSpeed = 1.5f;

    protected NavMeshAgent agent;
    protected Vector3 spawnPosition;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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

        UpdateBehaviour();
    }

    // Each NPC type defines its own behaviour.
    protected abstract void UpdateBehaviour();

    protected virtual void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    protected virtual void StopMoving()
    {
        agent.ResetPath();
    }
}
