using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth :  MonoBehaviour, IDamageable{

    [SerializeField] private float maxHealth = 100f;
    private float current;

    private void Awake()
    {
        current = maxHealth;
    }

    public void ApplyDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        current -= amount;
        Debug.Log($"{name} took {amount} damage at {hitPoint}. Remaining: {current}");

        if (current <= 0f)
        {
            Debug.Log($"{name} died.");
            Destroy(gameObject);
        }
    }
}