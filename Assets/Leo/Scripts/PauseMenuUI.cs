using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseRoot; // the PauseMenu panel root

    [Header("Options")]
    [SerializeField] private bool pauseWithTimeScale = true;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused;

    private void Awake()
    {
        if (pauseRoot == null) pauseRoot = gameObject;
        SetPaused(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            SetPaused(!isPaused);
        }
    }

    public void OnResumeClicked()
    {
        SetPaused(false);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pauseRoot != null)
            pauseRoot.SetActive(paused);

        if (pauseWithTimeScale)
            Time.timeScale = paused ? 0f : 1f;

        // Optional: lock/unlock cursor for game
        //Cursor.visible = paused;
        //Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        // Safety: if object is disabled while paused, restore time
        if (pauseWithTimeScale)
            Time.timeScale = 1f;
    }
}
