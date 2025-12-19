using UnityEngine;

/// <summary>
/// Optional scene component you can add to any GameObject to force-install tower health + health bars.
/// Use this if you want explicit, inspector-drivennn setup instead of relying on RuntimeInitializeOnLoad.
/// </summary>
[ExecuteAlways]
public class AdrianTowerSetupInScene : MonoBehaviour
{
    [Header("Scan")]
    [SerializeField] private float scanDurationSeconds = 10f;
    [SerializeField] private float scanIntervalSeconds = 0.5f;
    [SerializeField] private bool debugLogs = true;

    [Header("Target by Tag (recommended)")]
    [Tooltip("Create one of these tags and assign it to your main tower root objects.")]
    [SerializeField] private string[] towerTags = new[] { "Tower" };

    [Header("Fallback: Target by Name")]
    [Tooltip("Leave empty to disable name fallback (recommended to avoid tagging posts/stands by accident).")]
    [SerializeField] private string[] nameStartsWith = new string[0];
    [Tooltip("Leave empty to disable name fallback (recommended to avoid tagging posts/stands by accident).")]
    [SerializeField] private string[] nameContains = new string[0];

    [Header("Cleanup")]
    [Tooltip("If enabled, removes tower-health components from objects that are NOT tagged as towers and are NOT controllable turrets.")]
    [SerializeField] private bool cleanupNonTowerObjects = true;

    private float endTime;
    private float nextScanTime;

    private void OnEnable()
    {
        endTime = Now() + Mathf.Max(0.5f, scanDurationSeconds);
        nextScanTime = 0f;
    }

    private void Update()
    {
        // In Edit Mode, keep scanning so the health bars are visible without entering Play Mode.
        if (Application.isPlaying && Now() >= endTime)
            return;

        if (Now() < nextScanTime)
            return;

        int added = InstallOnMatchingTowers();
        if (debugLogs && added > 0)
            Debug.Log($"AdrianTowerSetupInScene: Added {added} components this scan.");

        if (cleanupNonTowerObjects)
            CleanupNonTowers();

        nextScanTime = Now() + Mathf.Max(0.1f, scanIntervalSeconds);
    }

    private int InstallOnMatchingTowers()
    {
        int added = 0;

        // Tag-first
        if (towerTags != null)
        {
            for (int i = 0; i < towerTags.Length; i++)
            {
                string tag = towerTags[i];
                if (string.IsNullOrWhiteSpace(tag))
                    continue;
                if (!TagExists(tag))
                    continue;

                try
                {
                    GameObject[] tagged = GameObject.FindGameObjectsWithTag(tag);
                    foreach (GameObject go in tagged)
                    {
                        if (go == null)
                            continue;
                        GameObject root = go.transform != null && go.transform.root != null ? go.transform.root.gameObject : go;
                        added += EnsureTowerSurvivabilityOn(root);
                    }
                }
                catch
                {
                    // Tag doesn't exist; ignore.
                }
            }
        }

        // Name fallback
        Transform[] all = FindObjectsOfType<Transform>(true);
        foreach (Transform t in all)
        {
            if (t == null)
                continue;
            GameObject root = t.root != null ? t.root.gameObject : t.gameObject;
            if (root == null)
                continue;

            if (!LooksLikeTower(root.name))
                continue;

            added += EnsureTowerSurvivabilityOn(root);
        }

        return added;
    }

    private bool LooksLikeTower(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return false;

        if (nameStartsWith != null)
        {
            for (int i = 0; i < nameStartsWith.Length; i++)
            {
                string s = nameStartsWith[i];
                if (!string.IsNullOrEmpty(s) && objectName.StartsWith(s, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        if (nameContains != null)
        {
            for (int i = 0; i < nameContains.Length; i++)
            {
                string c = nameContains[i];
                if (!string.IsNullOrEmpty(c) && objectName.IndexOf(c, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
        }

        return false;
    }

    private int EnsureTowerSurvivabilityOn(GameObject go)
    {
        if (go == null)
            return 0;

        int added = 0;

        TowerHealth health = go.GetComponent<TowerHealth>();
        if (health == null)
        {
            health = go.AddComponent<TowerHealth>();
            added++;
        }

        if (go.GetComponent<WorldSpaceHealthBar>() == null)
        {
            go.AddComponent<WorldSpaceHealthBar>();
            added++;
        }

        // Put the damage script on the tower ROOT so it's easy to find/tune range in the inspector.
        // (Proximity damage doesn't require colliders on the tower.)
        TowerEnemyContactDamage dmg = go.GetComponent<TowerEnemyContactDamage>();
        if (dmg == null)
        {
            dmg = go.AddComponent<TowerEnemyContactDamage>();
            added++;
        }

        if (debugLogs && added > 0)
            Debug.Log($"AdrianTowerSetupInScene: Installed on '{go.name}'");

        return added;
    }

    private void CleanupNonTowers()
    {
        TowerHealth[] healths = FindObjectsOfType<TowerHealth>(true);
        foreach (TowerHealth h in healths)
        {
            if (h == null)
                continue;

            GameObject go = h.gameObject;
            if (go == null)
                continue;

            // Keep if tagged as tower OR is a controllable turret (we want health bars on turrets too).
            bool keep = IsTaggedTower(go) ||
                        go.GetComponent<FirstPersonTurretController>() != null ||
                        go.GetComponent<ClickableTurret>() != null;

            if (keep)
                continue;

            // Remove our components from non-tower objects (e.g., posts/stands).
            WorldSpaceHealthBar bar = go.GetComponent<WorldSpaceHealthBar>();
            if (bar != null)
                DestroyImmediate(bar);

            // Remove damage scripts from this object and its children (in case it attached to a child collider).
            TowerEnemyContactDamage[] dmg = go.GetComponentsInChildren<TowerEnemyContactDamage>(true);
            foreach (TowerEnemyContactDamage d in dmg)
            {
                if (d != null)
                    DestroyImmediate(d);
            }

            DestroyImmediate(h);
        }
    }

    private bool IsTaggedTower(GameObject go)
    {
        if (go == null || towerTags == null)
            return false;

        for (int i = 0; i < towerTags.Length; i++)
        {
            string tag = towerTags[i];
            if (string.IsNullOrWhiteSpace(tag))
                continue;
            if (!TagExists(tag))
                continue;
            try
            {
                if (go.CompareTag(tag))
                    return true;
            }
            catch
            {
                // Tag doesn't exist.
            }
        }

        return false;
    }

    private static bool TagExists(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return false;

#if UNITY_EDITOR
        // In the editor we can safely check the tag list without triggering Unity's "Tag is not defined" spam.
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] == tag)
                return true;
        }
        return false;
#else
        // In builds, tags are fixed; assume valid to avoid surprising behavior.
        return true;
#endif
    }

    private static float Now()
    {
        return Application.isPlaying ? Time.unscaledTime : Time.realtimeSinceStartup;
    }
}


