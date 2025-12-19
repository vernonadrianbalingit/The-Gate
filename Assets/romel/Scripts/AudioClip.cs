using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Audio playback helper
*/

public class AudioPlayer : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }
    }

    public void PlaySound(string soundName)
    {
        if (audioManager != null)
        {
            audioManager.Play(soundName);
        }
    }

    public void StopSound(string soundName)
    {
        if (audioManager != null)
        {
            audioManager.Stop(soundName);
        }
    }
}
