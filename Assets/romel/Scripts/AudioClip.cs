using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        // Find the AudioManager in the scene
        audioManager = FindObjectOfType<AudioManager>();
        
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }
    }

    /// <summary>
    /// Play a sound by name from the AudioManager
    /// </summary>
    public void PlaySound(string soundName)
    {
        if (audioManager != null)
        {
            audioManager.Play(soundName);
        }
    }

    /// <summary>
    /// Stop a sound by name from the AudioManager
    /// </summary>
    public void StopSound(string soundName)
    {
        if (audioManager != null)
        {
            audioManager.Stop(soundName);
        }
    }

    /// <summary>
    /// Play a one-shot sound by name from the AudioManager
    /// </summary>
    public void PlayOneShotSound(string soundName)
    {
        if (audioManager != null)
        {
            audioManager.PlayOneShot(soundName);
        }
    }
}
