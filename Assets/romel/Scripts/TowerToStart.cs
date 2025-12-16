using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerToStart : MonoBehaviour
{
    public GameObject UI;
    public GameObject SpawnLogic;
    public GameObject CurrencyHandler;

    private Transform mainCamera;
    private Transform currentCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null)
        {
            return;
        }
        
        currentCamera = Camera.main.transform;
        if (currentCamera != mainCamera && mainCamera != null)
        {
            UI.SetActive(true);
            SpawnLogic.SetActive(true);
            CurrencyHandler.SetActive(true);
            Destroy(gameObject);
        }
    }

    
}
