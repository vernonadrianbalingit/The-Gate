using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCityEnter : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ScrCityHealth cityHealth = FindObjectOfType<ScrCityHealth>();
            if (cityHealth != null)
            {
                cityHealth.cityHealth -= 10; // Decrease city health by 10 on collision
                Debug.Log("City Health: " + cityHealth.cityHealth);
            }
            Destroy(collision.gameObject); // Destroy the enemy after it hits the city
        }
    }
}
