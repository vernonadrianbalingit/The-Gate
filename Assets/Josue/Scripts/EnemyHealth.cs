using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float deathDelay = 1.5f;

    private float current;
    private Animator animator;
    private bool isDead = false;

    private Renderer rend;
    private Color originalColor;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashTime = 0.1f;

    private void Awake()
    {
        current = maxHealth;
        animator = GetComponent<Animator>();

        rend = GetComponentInChildren<Renderer>();
        /*if (rend != null)
        {
            originalColor = rend.material.color;
        }*/
    }

    public void ApplyDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return;

        current -= amount;
        Debug.Log($"{name} took {amount} damage at {hitPoint}. Remaining: {current}");

        if (current > 0f)
        {
            if (animator != null)
                animator.SetTrigger("Take Damage");

            if (rend != null)
                StartCoroutine(FlashRed());
        }

        // Death
        if (current <= 0f)
        {
            isDead = true;

            if (animator != null)
                animator.SetTrigger("Die");

            Debug.Log($"{name} died.");

            // Wait for death animation before destroying
            StartCoroutine(DieAfterAnimation());
        }
    }

    private IEnumerator DieAfterAnimation()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);

    }

    private IEnumerator FlashRed()
    {
        rend.material.color = hitColor;
        yield return new WaitForSeconds(hitFlashTime);
        rend.material.color = originalColor;
    }
}
