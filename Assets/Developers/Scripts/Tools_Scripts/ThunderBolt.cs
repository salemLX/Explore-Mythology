
using System.Collections.Generic;
using UnityEngine;

// Author: Salem
// Created: 24-11-2026
// Description: Thunderbolt impact, damage, panic and effects.

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Thunderbolt : MonoBehaviour
{
    [Header("Impact Settings")]
    [SerializeField] private float minimumImpactSpeed = 5f;
    [SerializeField] private int damage = 25;

    [Header("Area Damage")]
    [SerializeField] private float damageRadius = 4f;
    [SerializeField] private LayerMask damageLayers = ~0;

    [Header("NPC Panic")]
    [SerializeField] private float panicRadius = 8f;
    [SerializeField] private LayerMask npcLayers;

    [Header("Effects")]
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject burnMarkPrefab;

    [Header("Effect Lifetime")]
    [SerializeField] private float effectLifetime = 3f;
    [SerializeField] private float burnMarkLifetime = 30f;

    private bool hasExploded;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded || collision.contactCount == 0)
            return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed < minimumImpactSpeed)
            return;

        hasExploded = true;

        ContactPoint contact = collision.GetContact(0);
        Vector3 impactPosition = contact.point;

        // Prevent damage being applied twice to one target.
        HashSet<IDamageable> damagedTargets = new();

        // Direct impact damage.
        ApplyDamage(collision.collider, damagedTargets);

        // Area damage.
        DamageNearby(impactPosition, damagedTargets);

        // Make nearby NPCs panic.
        AlertNearbyNPCs(impactPosition);

        // Show impact ranges in the Scene view.
        DrawDebugCircle(impactPosition, damageRadius, Color.red);
        DrawDebugCircle(impactPosition, panicRadius, Color.yellow);

        Quaternion surfaceRotation =
            Quaternion.FromToRotation(
                Vector3.up,
                contact.normal
            );

        Vector3 spawnPosition =
            contact.point + contact.normal * 0.02f;

        // Impact effect.
        if (impactEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                impactEffectPrefab,
                spawnPosition,
                surfaceRotation
            );

            Destroy(effect, effectLifetime);
        }

        // Burn mark.
        if (burnMarkPrefab != null && !collision.collider.CompareTag("NPC"))
        {
            GameObject mark = Instantiate(
                burnMarkPrefab,
                spawnPosition,
                surfaceRotation
            );

            Destroy(mark, burnMarkLifetime);
        }

        Destroy(gameObject);
    }

    private void DamageNearby(
        Vector3 impactPosition,
        HashSet<IDamageable> damagedTargets)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(
            impactPosition,
            damageRadius,
            damageLayers,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider nearbyObject in nearbyObjects)
        {
            ApplyDamage(nearbyObject, damagedTargets);
        }
    }

    private void ApplyDamage(
        Collider hitCollider,
        HashSet<IDamageable> damagedTargets)
    {
        IDamageable target =
            hitCollider.GetComponentInParent<IDamageable>();

        if (target == null || !damagedTargets.Add(target))
            return;

        target.TakeDamage(damage);

        Debug.Log(
            $"{hitCollider.name} received {damage} damage!"
        );
    }

    private void AlertNearbyNPCs(Vector3 impactPosition)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(
            impactPosition,
            panicRadius,
            npcLayers,
            QueryTriggerInteraction.Collide
        );

        HashSet<IPanicable> alertedNPCs = new();

        foreach (Collider nearbyObject in nearbyObjects)
        {
            IPanicable npc =
                nearbyObject.GetComponentInParent<IPanicable>();

            if (npc != null && alertedNPCs.Add(npc))
            {
                npc.Panic(impactPosition);
            }
        }
    }

    // Visual debugging after impact.
    private void DrawDebugCircle(
        Vector3 center, float radius, Color color)
    {
        const int segments = 48;

        for (int i = 0; i < segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            float b = (i + 1) * Mathf.PI * 2f / segments;

            Vector3 start = center + new Vector3(
                Mathf.Cos(a) * radius, 0f,
                Mathf.Sin(a) * radius
            );

            Vector3 end = center + new Vector3(
                Mathf.Cos(b) * radius, 0f,
                Mathf.Sin(b) * radius
            );

            Debug.DrawLine(start, end, color, 4f);
        }
    }

    // Preview the ranges in the Unity Editor.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, panicRadius);
    }
}
