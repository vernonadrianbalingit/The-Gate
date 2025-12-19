using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Simple Script to activate game functions on selection of first tower
- Deletes itself after activation
*/

public class TowerToStart : MonoBehaviour
{
    public GameObject UI;
    public GameObject SpawnLogic;
    public GameObject CurrencyHandler;

    private Transform mainCamera;
    private Transform currentCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        if (Camera.main == null)
        {
            return;
        }
        
        currentCamera = Camera.main.transform;

        // checks if the current camera is different from the main camera
        if (currentCamera != mainCamera && mainCamera != null)
        {
            UI.SetActive(true);
            SpawnLogic.SetActive(true);
            CurrencyHandler.SetActive(true);
            Destroy(gameObject);
        }
    }

    
}
