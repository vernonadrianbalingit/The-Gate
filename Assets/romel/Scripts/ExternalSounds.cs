using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalSounds : MonoBehaviour
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
    // Update is called once per frame
    void Update()
    {
        // when tab is pressed, play sound
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            
            if (audioManager != null)
            {
                audioManager.Play("MenuClick");
            }
        }
    }
}
