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
        // Handle Pause Input
        if (Input.GetKeyDown(KeyCode.Escape))
    {
        // Only allow pausing if the game is active (not on the initial Start screen 
        // and not already Game Over).
        if (gameManager.CurrentState != GameManager.GameState.AwaitStart && 
            gameManager.CurrentState != GameManager.GameState.GameOver)
        {
            TogglePause();
        }
    }
    if (pauseScreenPanel.activeSelf)
    {
        return; // <--- THIS LINE IS ABSOLUTELY ESSENTIAL!
    }
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
            case GameManager.GameState.PlayerSlotDecision:
                ShowSlotDecisionUI(); 
                break;
            case GameManager.GameState.PlayerAbilitySelect:
                ShowAbilitySelectionUI();
                break;
            default:
                // Ensure Game UI is visible during active play states
                ShowGameUI();
                break;
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
        if (gameManager != null)
    {
        gameManager.InitializeGame(); 
    }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

        public void ShowSlotDecisionUI()
    {
    // 1. Ensure the main game HUD is visible (hides Start/Pause/Game Over screens)
    ShowGameUI(); 

    // 2. Activate the Slot Decision Panel
    slotDecisionPanel.SetActive(true); 

    // 3. Deactivate any other phase-specific panels
    abilitySelectionPanel.SetActive(false); 
    }   

    public void ShowAbilitySelectionUI()
    {
    // 1. Ensure the main game HUD is visible
    ShowGameUI(); 

    // 2. Activate the Ability Selection Panel
    abilitySelectionPanel.SetActive(true);

    // 3. Deactivate the Slot Decision Panel
    slotDecisionPanel.SetActive(false);
    }
}