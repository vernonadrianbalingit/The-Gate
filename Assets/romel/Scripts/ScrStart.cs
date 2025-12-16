using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrStart : MonoBehaviour
{
    public GameObject UI;
    public GameObject SpawnLogic;
    
    public GameObject CurrencyHandler;
    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartButtonPressed()
    {
        UI.SetActive(true);
        SpawnLogic.SetActive(true);
        CurrencyHandler.SetActive(true);
        gameObject.SetActive(false);
    }
}
