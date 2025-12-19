using UnityEngine;

/// <summary>
/// This component is deprecated - damage detection is now handled by WorldSpaceHealthBar.
/// Keeping this file for backward compatibility but it does nothing.
/// You can safely remove this component from your towers.
/// </summary>
public class TowerEnemyContactDamage : MonoBehaviour
{
    void Start()
    {
        Debug.LogWarning("TowerEnemyContactDamage is deprecated. Damage is now handled by WorldSpaceHealthBar. You can remove this component.");
    }
}
