using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";  

    [Header("Settings Panel (Optional)")]
    [SerializeField] private GameObject settingsPanel;

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsButtonClicked()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
        else
        {
            Debug.Log("Settings panel not hooked up yet.");
        }
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit Game");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
