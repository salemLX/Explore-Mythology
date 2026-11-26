

//Author: Thay
//Created: 26-11-2026


// Tablet screen startup
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SecondScreenSetup : MonoBehaviour
{
    [Tooltip("Exact name of the arena scene (must be in Build Settings)")]
    [SerializeField] string arenaSceneName = "Tests_2D Arena";

    [Tooltip("Load the arena automatically next to the VR scene")]
    [SerializeField] bool loadArenaAdditively = true;

    void Awake()
    {
        // Display 1 is always active. Turn on every extra display (the tablet).
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        Debug.Log($"Displays connected: {Display.displays.Length}");

        if (loadArenaAdditively)
        {
            SceneManager.LoadSceneAsync(arenaSceneName, LoadSceneMode.Additive);
        }
    }
    IEnumerator Start()
    {
        // Wait so the arena has time to finish loading
        yield return new WaitForSeconds(2f);

        foreach (var c in Camera.allCameras)
        {
            Debug.Log($"Camera: {c.name}, display {c.targetDisplay + 1}, enabled: {c.enabled}");
        }
    }

}
