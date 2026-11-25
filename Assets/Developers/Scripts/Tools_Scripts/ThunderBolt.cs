using UnityEngine;


// Author: Salem
// Created: [24-11-2026]



// Class representing a thunderbolt projectile
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Thunderbolt : MonoBehaviour
{
    [Header("Impact Settings")]
    [SerializeField] private float minimumImpactSpeed = 5f;
    [SerializeField] private int damage = 25;

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

        // Damage
        IDamageable target =
            collision.collider.GetComponentInParent<IDamageable>();

        target?.TakeDamage(damage);

        // Align local Y axis with the surface normal.
        Quaternion surfaceRotation =
            Quaternion.FromToRotation(
                Vector3.up,
                contact.normal
            );

        Vector3 spawnPosition =
            contact.point + contact.normal * 0.02f;

        // Impact effect
        if (impactEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                impactEffectPrefab,
                spawnPosition,
                surfaceRotation
            );

            Destroy(effect, effectLifetime);
        }

        // Burn mark
        if (burnMarkPrefab != null)
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
}
