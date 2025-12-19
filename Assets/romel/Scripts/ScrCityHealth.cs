using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
- Manages the health of the city
- Updates health bar UI
- Reloads scene when health reaches zero
*/ 

public class ScrCityHealth : MonoBehaviour
{
    public Image healthBar;
    public int cityHealth = 100;
    void Update()
    {
        if (healthBar != null)
            healthBar.fillAmount = cityHealth / 100f;

        if (cityHealth <= 0)
        {
            SceneManager.LoadScene("Map1");
        }
    }
}
