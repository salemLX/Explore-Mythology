using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Salem
// Created: 26-11-2026


// Description: Visual feedback when an NPC takes damage.
public class NPCDamageFlash : MonoBehaviour
{
    [Header("Damage Flash")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float fadeDuration = 0.5f;

    private readonly List<Material> materials = new();
    private readonly List<Color> originalColors = new();
    private readonly List<int> colorProperties = new();

    private Coroutine flashCoroutine;

    private void Awake()
    {
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            if (renderer is not MeshRenderer &&
                renderer is not SkinnedMeshRenderer)
                continue;

            foreach (Material mat in renderer.materials)
            {
                if (mat == null)
                    continue;

                int property = mat.HasProperty("_BaseColor")
                    ? Shader.PropertyToID("_BaseColor")
                    : Shader.PropertyToID("_Color");

                if (!mat.HasProperty(property))
                    continue;

                materials.Add(mat);
                colorProperties.Add(property);
                originalColors.Add(mat.GetColor(property));
            }
        }
    }


    // Method to trigger the damage flash effect.
    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fadeDuration);

            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].SetColor(
                    colorProperties[i],
                    Color.Lerp(
                        damageColor,
                        originalColors[i],
                        t
                    )
                );
            }

            yield return null;
        }

        flashCoroutine = null;
    }

    // Clean up instantiated materials to prevent memory leaks.
    private void OnDestroy()
    {
        foreach (Material mat in materials)
            Destroy(mat);
    }


    public void SetDeathColor(float redAmount = 0.5f)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetColor(
                colorProperties[i],
                Color.Lerp(
                    originalColors[i],
                    damageColor,
                    Mathf.Clamp01(redAmount)
                )
            );
        }
    }

}
