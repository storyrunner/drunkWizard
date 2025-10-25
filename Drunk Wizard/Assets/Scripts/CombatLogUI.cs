using UnityEngine;
using TMPro;

public class CombatLogUI : MonoBehaviour
{
    public TextMeshProUGUI logText;
    private GameManager gameManager;
    private string lastLogMessage = "";

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (logText == null) logText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (gameManager.combatLogMessage != lastLogMessage)
        {
            logText.text = gameManager.combatLogMessage;
            lastLogMessage = gameManager.combatLogMessage;
        }
    }
}