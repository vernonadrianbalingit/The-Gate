using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnCityEnter : MonoBehaviour
{

    public Image healthBar;

    public int cityHealthValue = 100;

    void Update()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = cityHealthValue / 100f;
        }

        if (cityHealthValue <= 0)
        {
            SceneManager.LoadScene("Map1");
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            cityHealthValue -= 10; // Decrease city health by 10 on collision
            Debug.Log("City Health: " + cityHealthValue);
            Destroy(collision.gameObject); // Destroy the enemy after it hits the city
        }
    }
}
