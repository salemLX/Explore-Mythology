using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Author: Salem
// Created: 26-11-2026
// Description: Handles NPC death animations, effects and cleanup.

public class NPCDeath : MonoBehaviour
{
    [Header("Death Animation")]
    [SerializeField] private Transform npcBody;
    [SerializeField] private float deathRotation = -90f;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float disappearDelay = 2f;

    [Header("Death Visuals")]
    [SerializeField] private NPCDamageFlash damageFlash;
    [SerializeField] private GameObject panicIndicator;

    [Header("Death Audio")]
    [SerializeField] private AudioClip[] deathSounds;

    private bool isDead;

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Stop NPC behaviour.
        NPCBase npc = GetComponent<NPCBase>();

        if (npc != null)
            npc.enabled = false;

        // Stop navigation.
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.enabled = false;

        // Disable all colliders.
        foreach (Collider col in
                 GetComponentsInChildren<Collider>(true))
        {
            col.enabled = false;
        }

        // Disable gravity and physics movement.
        foreach (Rigidbody rb in
                 GetComponentsInChildren<Rigidbody>(true))
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Stop animations.
        foreach (Animator animator in
                 GetComponentsInChildren<Animator>(true))
        {
            animator.enabled = false;
        }

        // Hide panic indicator.
        if (panicIndicator != null && panicIndicator != gameObject && panicIndicator.transform.IsChildOf(transform))
        {
            panicIndicator.SetActive(false);
        }

        // Stop existing panic sounds.
        AudioSource audio = GetComponent<AudioSource>();

        if (audio != null)
            audio.Stop();

        // Keep the body partially red.
        if (damageFlash != null)
            damageFlash.SetDeathColor(0.5f);

        // Play a random death sound.
        if (deathSounds.Length > 0)
        {
            AudioClip clip = deathSounds[
                Random.Range(0, deathSounds.Length)
            ];

            if (clip != null)
                AudioSource.PlayClipAtPoint(
                    clip, transform.position
                );
        }

        StartCoroutine(DeathAnimation());

    }

    private IEnumerator DeathAnimation()
    {
        if (npcBody != null)
        {
            Quaternion startRotation = npcBody.localRotation;

            Quaternion endRotation = startRotation *
                Quaternion.Euler(0f, 0f, deathRotation);

            float timer = 0f;

            while (timer < rotationDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(
                    timer / Mathf.Max(0.01f, rotationDuration)
                );

                t = Mathf.SmoothStep(0f, 1f, t);

                npcBody.localRotation = Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    t
                );

                yield return null;
            }

            npcBody.localRotation = endRotation;
        }

        yield return new WaitForSeconds(disappearDelay);

        Destroy(gameObject);
    }
}
