using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrCityHealth : MonoBehaviour
{
    public int cityHealth = 100;
    void Update()
    {
        if (cityHealth <= 0)
        {
            SceneManager.LoadScene("Map1");
        }
    }
}
