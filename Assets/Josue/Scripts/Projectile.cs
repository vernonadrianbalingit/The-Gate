using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class Projectile : MonoBehaviour{

    [SerializeField] private float speed = 40f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeSeconds = 5f;
    [SerializeField] private LayerMask hitMask;

    private Rigidbody rb;
    private string ownerTag;
    private bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
    }

    public void Initialize(Vector3 direction, float damageOverride, string ownerTag)
    {
        this.ownerTag = ownerTag;
        if (damageOverride > 0f) damage = damageOverride;

        initialized = true;
        rb.velocity = direction.normalized * speed;
        CancelInvoke();
        Invoke(nameof(Despawn), lifeSeconds);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!initialized) return;

        
        if (!LayerIncluded(hitMask, other.gameObject.layer)) return;
        if (other.CompareTag(ownerTag)) return;

        // Hit logging + damage application
        Debug.Log($"Projectile hit {other.name}");

        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            Vector3 p = other.ClosestPoint(transform.position);
            Vector3 n = (transform.position - p).normalized;
            dmg.ApplyDamage(damage, p, n);
        }

        Despawn();
    }

    private bool LayerIncluded(LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
