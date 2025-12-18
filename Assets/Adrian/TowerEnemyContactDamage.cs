using UnityEngine;

/// <summary>
/// Makes a tower take damage over time when an enemy (e.g. bee) is in contact.
/// Works via triggers OR collisions, so it doesn't require changing enemy scripts.
/// </summary>
[DisallowMultipleComponent]
public class TowerEnemyContactDamage : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TowerHealth towerHealth;

    [Header("Enemy Detection")]
    [Tooltip("If set, only these tags are treated as enemies. If empty, name-based fallback will be used.")]
    [SerializeField] private string[] enemyTags = new[] { "Enemy" };
    [Tooltip("If a collider's name (or root name) contains this, it will be treated as an enemy.")]
    [SerializeField] private string nameContainsFallback = "bee";

    [Header("Damage")]
    [SerializeField] private float damagePerSecond = 12f;
    [SerializeField] private float tickInterval = 0.25f;

    [Header("Proximity Damage (recommended)")]
    [Tooltip("If enabled, tower takes damage when enemies are within radius (no collision required).")]
    [SerializeField] private bool useProximityDamage = true;
    [SerializeField] private float proximityRadius = 3.5f;
    [Tooltip("Optional layer mask to reduce physics checks. Leave as Everything if unsure.")]
    [SerializeField] private LayerMask enemyLayerMask = ~0;
    [SerializeField] private bool countMultipleEnemies = true; // damage scales with # of enemies in range
    [SerializeField] private int maxEnemiesCounted = 10;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    private float nextTickTime;
    private readonly Collider[] overlapBuffer = new Collider[32];

    private void OnValidate()
    {
#if UNITY_EDITOR
        // Prevent Unity console spam: CompareTag logs an error if the tag doesn't exist,
        // even if we try/catch around it. So we sanitize the list in the editor.
        if (enemyTags == null || enemyTags.Length == 0)
            return;

        int write = 0;
        for (int read = 0; read < enemyTags.Length; read++)
        {
            string t = enemyTags[read];
            if (string.IsNullOrWhiteSpace(t))
                continue;
            if (!TagExists(t))
                continue;
            enemyTags[write++] = t.Trim();
        }

        if (write == enemyTags.Length)
            return;

        string[] trimmed = new string[write];
        for (int i = 0; i < write; i++)
            trimmed[i] = enemyTags[i];
        enemyTags = trimmed;
#endif
    }

    /// <summary>
    /// Allows installers to wire this damage trigger to a TowerHealth that may live on a parent/root.
    /// </summary>
    public void SetTowerHealth(TowerHealth health)
    {
        towerHealth = health;
    }

    private void Reset()
    {
        towerHealth = GetComponent<TowerHealth>();
    }

    private void Awake()
    {
        if (towerHealth == null)
            towerHealth = GetComponent<TowerHealth>();

        // Common setup: collider lives on a child, health lives on the root/parent.
        if (towerHealth == null)
            towerHealth = GetComponentInParent<TowerHealth>();

        if (towerHealth == null)
            towerHealth = GetComponentInChildren<TowerHealth>();
    }

    private void Update()
    {
        if (!useProximityDamage)
            return;

        if (towerHealth == null || towerHealth.IsDead)
            return;

        if (proximityRadius <= 0f)
            return;

        if (Time.time < nextTickTime)
            return;

        Vector3 center = GetDamageCenter();
        int hits = Physics.OverlapSphereNonAlloc(center, proximityRadius, overlapBuffer, enemyLayerMask, triggerInteraction);
        if (hits <= 0)
        {
            nextTickTime = Time.time + tickInterval;
            return;
        }

        int enemies = 0;
        for (int i = 0; i < hits; i++)
        {
            Collider c = overlapBuffer[i];
            if (c == null)
                continue;
            if (!IsEnemy(c))
                continue;
            enemies++;
            if (!countMultipleEnemies)
                break;
            if (enemies >= maxEnemiesCounted)
                break;
        }

        if (enemies > 0)
        {
            float dmg = damagePerSecond * tickInterval * (countMultipleEnemies ? enemies : 1);
            towerHealth.TakeDamage(dmg);
        }

        nextTickTime = Time.time + tickInterval;
    }

    private void OnTriggerStay(Collider other)
    {
        TryApplyDamage(other);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision == null)
            return;
        TryApplyDamage(collision.collider);
    }

    private void TryApplyDamage(Collider other)
    {
        if (towerHealth == null || towerHealth.IsDead)
            return;

        if (other == null)
            return;

        if (!IsEnemy(other))
            return;

        if (Time.time < nextTickTime)
            return;

        float dmg = damagePerSecond * tickInterval;
        towerHealth.TakeDamage(dmg, other.gameObject);
        nextTickTime = Time.time + tickInterval;
    }

    private Vector3 GetDamageCenter()
    {
        // Prefer anchoring to the visible mesh bounds if possible (tower pivots are sometimes underground).
        Renderer r = towerHealth != null ? towerHealth.GetComponentInChildren<Renderer>() : null;
        if (r != null)
            return r.bounds.center;
        return towerHealth != null ? towerHealth.transform.position : transform.position;
    }

    private bool IsEnemy(Collider other)
    {
        // Prefer tag checks if tags are configured and exist.
        if (enemyTags != null && enemyTags.Length > 0)
        {
            for (int i = 0; i < enemyTags.Length; i++)
            {
                string t = enemyTags[i];
                if (string.IsNullOrWhiteSpace(t))
                    continue;

                // IMPORTANT: CompareTag logs an error if tag doesn't exist (even if caught).
                // So we must check tag existence BEFORE calling CompareTag.
                if (!TagExists(t))
                    continue;

                // Check collider, then parents/root (your bees have tag on root but colliders on children).
                if (other.CompareTag(t))
                    return true;

                Transform tr = other.transform;
                while (tr != null)
                {
                    if (tr.CompareTag(t))
                        return true;
                    tr = tr.parent;
                }

                Transform root = other.transform != null ? other.transform.root : null;
                if (root != null && root.CompareTag(t))
                    return true;
            }
        }

        // Fallback: name contains (case-insensitive)
        if (!string.IsNullOrWhiteSpace(nameContainsFallback))
        {
            string needle = nameContainsFallback.Trim().ToLowerInvariant();

            string n1 = other.name != null ? other.name.ToLowerInvariant() : "";
            if (n1.Contains(needle))
                return true;

            Transform root = other.transform != null ? other.transform.root : null;
            if (root != null)
            {
                string n2 = root.name != null ? root.name.ToLowerInvariant() : "";
                if (n2.Contains(needle))
                    return true;
            }
        }

        return false;
    }

    private static bool TagExists(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return false;

#if UNITY_EDITOR
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] == tag)
                return true;
        }
        return false;
#else
        // In builds, tag list is fixed. Assume valid to avoid unexpected behavior.
        return true;
#endif
    }
}


