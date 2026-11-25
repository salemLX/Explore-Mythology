using UnityEngine;
using UnityEngine.AI;


// Author: Salem
// Created: [24-11-2026]


// Class representing an NPC that roams around a specified area,Inheritated from NPCBase
public class NPCRoaming : NPCBase
{
    [Header("Roaming")]
    [SerializeField] private float roamRadius = 8f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;

    private float nextMoveTime;
    private bool isMoving;

    protected override void Start()
    {
        base.Start();
        ScheduleNextMove();
    }

    protected override void UpdateBehaviour()
    {
        if (isMoving)
        {
            if (agent.pathPending)
                return;

            if (!agent.hasPath ||
                agent.remainingDistance <=
                agent.stoppingDistance + 0.15f)
            {
                isMoving = false;
                StopMoving();
                ScheduleNextMove();
            }

            return;
        }

        if (Time.time >= nextMoveTime)
            FindRandomDestination();
    }

    private void FindRandomDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 random = Random.insideUnitCircle
                             * roamRadius;

            Vector3 target = spawnPosition +
                new Vector3(random.x, 0f, random.y);

            if (!NavMesh.SamplePosition(
                target,
                out NavMeshHit hit,
                2f,
                agent.areaMask))
                continue;

            NavMeshPath path = new NavMeshPath();

            if (!agent.CalculatePath(hit.position, path))
                continue;

            if (path.status != NavMeshPathStatus.PathComplete)
                continue;

            if (Vector3.Distance(
                transform.position, hit.position) < 1f)
                continue;

            MoveTo(hit.position);
            isMoving = true;
            return;
        }

        ScheduleNextMove();
    }

    private void ScheduleNextMove()
    {
        nextMoveTime = Time.time +
            Random.Range(minWaitTime, maxWaitTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 center = Application.isPlaying
            ? spawnPosition
            : transform.position;

        Gizmos.DrawWireSphere(center, roamRadius);
    }
}
