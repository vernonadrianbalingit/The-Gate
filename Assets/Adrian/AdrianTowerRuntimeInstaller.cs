using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Scene-agnostic runtime installer to ensure "main towers" (e.g., Tower_Base...) get health + health bars.
/// This runs even if the tower objects are not Adrian prefabs and/or are spawned at runtime.
/// </summary>
public class AdrianTowerRuntimeInstaller : MonoBehaviour
{
    [SerializeField] private float scanDurationSeconds = 8f;
    [SerializeField] private float scanIntervalSeconds = 0.5f;
    [SerializeField] private bool debugLogs = false;

    [Header("Targeting (name-based)")]
    [Tooltip("Leave empty to disable name fallback (recommended to avoid posts/stands).")]
    [SerializeField] private string[] nameStartsWith = new string[0];
    [Tooltip("Leave empty to disable name fallback (recommended to avoid posts/stands).")]
    [SerializeField] private string[] nameContains = new string[0];

    [Header("Targeting (tag-based)")]
    [Tooltip("If these tags exist, any object with one of these tags will be treated as a tower root.")]
    [SerializeField] private string[] towerTags = new[] { "Tower" };

    private static bool created;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (created)
            return;
        created = true;

        GameObject go = new GameObject("AdrianTowerRuntimeInstaller");
        DontDestroyOnLoad(go);
        // Keep visible so you can confirm it exists while debugging.
        // (You can collapse it in the hierarchy; it won't affect gameplay.)
        go.hideFlags = HideFlags.DontSave;
        go.AddComponent<AdrianTowerRuntimeInstaller>();
    }

    private void Start()
    {
        StartCoroutine(ScanLoop());
    }

    private IEnumerator ScanLoop()
    {
        float endTime = Time.unscaledTime + Mathf.Max(0.5f, scanDurationSeconds);

        while (Time.unscaledTime < endTime)
        {
            int added = InstallOnMatchingTowers();
            if (debugLogs && added > 0)
                Debug.Log($"AdrianTowerRuntimeInstaller: Added {added} components this scan.");

            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, scanIntervalSeconds));
        }

        if (debugLogs)
            Debug.Log("AdrianTowerRuntimeInstaller: Scan loop finished.");
    }

    private int InstallOnMatchingTowers()
    {
        int added = 0;

        // Tag-first (fast path)
        if (towerTags != null && towerTags.Length > 0)
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

        Transform[] all = FindObjectsOfType<Transform>(true);
        foreach (Transform t in all)
        {
            if (t == null)
                continue;

            GameObject root = t.root != null ? t.root.gameObject : t.gameObject;
            if (root == null)
                continue;

            if (!LooksLikeMainTower(root.name))
                continue;

            added += EnsureTowerSurvivabilityOn(root);
        }

        return added;
    }

    private bool LooksLikeMainTower(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return false;

        for (int i = 0; i < nameStartsWith.Length; i++)
        {
            string s = nameStartsWith[i];
            if (!string.IsNullOrEmpty(s) && objectName.StartsWith(s, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        for (int i = 0; i < nameContains.Length; i++)
        {
            string c = nameContains[i];
            if (!string.IsNullOrEmpty(c) && objectName.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
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
        return true;
#endif
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
            dmg.SetTowerHealth(health);
            added++;
        }
        else if (health != null)
        {
            // Ensure it points at the tower's health (in case it was added to a child collider).
            dmg.SetTowerHealth(health);
        }

        if (debugLogs && added > 0)
            Debug.Log($"AdrianTowerRuntimeInstaller: Installed on '{go.name}'");

        return added;
    }
}


