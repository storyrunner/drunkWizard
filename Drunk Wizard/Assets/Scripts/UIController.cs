using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{
    [Header("Screen Panels")]
    public GameObject startScreenPanel;
    public GameObject pauseScreenPanel;
    public GameObject gameOverScreenPanel;

    [Header("Phase-Specific UI")]
    public GameObject inGameHUDPanel; // Contains Health Bars, Log, etc.
    public GameObject slotDecisionPanel; // The 3 rolled abilities + swap buttons
    public GameObject abilitySelectionPanel; // The 5 permanent slots + Confirm Chain button

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! UI will not function.");
            enabled = false;
            return;
        }

        ShowStartScreen();
    }

    void Update()
    {
        // Control visibility based on GameState
        // Note: Slot and Ability panels are controlled within their respective scripts for granularity.
        switch (gameManager.CurrentState)
        {
            case GameManager.GameState.AwaitStart:
                // Do nothing, Start Screen is visible
                break;
            case GameManager.GameState.GameOver:
                ShowGameOverScreen();
                break;
            default:
                // Ensure Game UI is visible during active play states
                if (!inGameHUDPanel.activeSelf) ShowGameUI();
                break;
        }

        // Handle Pause Input
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.CurrentState != GameManager.GameState.GameOver)
        {
            TogglePause();
        }
    }

    // --- Public Screen Control Methods (Called by UI Buttons) ---

    public void ShowStartScreen()
    {
        startScreenPanel.SetActive(true);
        pauseScreenPanel.SetActive(false);
        gameOverScreenPanel.SetActive(false);
        inGameHUDPanel.SetActive(false);
        Time.timeScale = 0;
    }

    public void StartGameClicked()
    {
        gameManager.StartGame(); // Assuming you add this simple method to GameManager
        ShowGameUI();
        Time.timeScale = 1;
    }

    public void ShowGameUI()
    {
        startScreenPanel.SetActive(false);
        pauseScreenPanel.SetActive(false);
        gameOverScreenPanel.SetActive(false);
        inGameHUDPanel.SetActive(true);
    }

    public void TogglePause()
    {
        bool isPaused = pauseScreenPanel.activeSelf;
        pauseScreenPanel.SetActive(!isPaused);
        Time.timeScale = isPaused ? 1 : 0;
    }

    public void ShowGameOverScreen()
    {
        gameOverScreenPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}