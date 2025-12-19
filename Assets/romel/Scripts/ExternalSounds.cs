using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Sounds for audio with no source
*/

public class ExternalSounds : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
                audioManager.Play("MenuClick");
        }
    }
}
