using System;
using UnityEngine;

/// <summary>
/// Simple health component for towers. Supports being damaged by enemies (bees) and
/// drives optional healthbar updates via events.
/// </summary>
[DisallowMultipleComponent]
public class TowerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 200f;
    [SerializeField] private float startingHealth = 200f;
    [SerializeField] private bool destroyOnDeath = false;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action<float, float> OnHealthChanged; // (current, max)
    public event Action OnDied;

    private float currentHealth;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        startingHealth = Mathf.Clamp(startingHealth, 0f, maxHealth);
        currentHealth = startingHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Convenience method for other scripts that call TakeDamage(float).
    /// </summary>
    public void TakeDamage(float amount)
    {
        TakeDamage(amount, null);
    }

    public void TakeDamage(float amount, GameObject source)
    {
        if (IsDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            OnDied?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetMaxHealth(float newMax, bool fillToMax = true)
    {
        maxHealth = Mathf.Max(1f, newMax);
        if (fillToMax)
            currentHealth = maxHealth;
        else
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        startingHealth = Mathf.Clamp(startingHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}


