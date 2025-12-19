using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
- Handles city health and enemy collisions
- Updates health bar UI
- deletes enemy on collision
- Reloads scene when health reaches zero
*/

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
            cityHealthValue -= 10;
            Debug.Log("City Health: " + cityHealthValue);
            Destroy(collision.gameObject);
        }
    }
}
