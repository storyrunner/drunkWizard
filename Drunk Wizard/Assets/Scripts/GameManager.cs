using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        AwaitStart,
        SlotPhase,
        PlayerSlotDecision,
        PlayerAbilitySelect,
        EnemyAction,
        GameOver
    }

    private GameState currentState = GameState.AwaitStart;

    public AbilitySlotMachine playerRoller;
    public AbilitySlotMachine enemyRoller;
    public PlayerController player; 
    public EnemyAI enemy; 

    public Ability playerRolledAbility; 
    public Ability enemyRolledAbility;
    public string combatLogMessage = "";



    void Awake()
    {
        // 1. Find Player and Enemy characters (always safe using FindFirstObjectByType)
        player = FindFirstObjectByType<PlayerController>();
        enemy = FindFirstObjectByType<EnemyAI>();
        
        // 2. Find ALL slot machine components in the scene
        // We use FindFirstObjectByType to ensure we get both machines, even if they are children of other objects.
        AbilitySlotMachine[] rollers = FindObjectsOfType<AbilitySlotMachine>();

        // 3. Assign the rollers based on naming convention
        foreach (var roller in rollers)
        {
            if (roller.gameObject.name.Contains("Player"))
            {
                playerRoller = roller;
            }
            else if (roller.gameObject.name.Contains("Enemy"))
            {
                enemyRoller = roller;
            }
        }

        // 4. Subscribe to the events
        if (playerRoller != null) playerRoller.OnAbilityRolled += OnPlayerAbilityRolled;
        if (enemyRoller != null) enemyRoller.OnAbilityRolled += OnEnemyAbilityRolled;
        
        // Final sanity check
        if (playerRoller == null || enemyRoller == null || player == null || enemy == null)
        {
            Debug.LogError("FATAL ERROR: Core game components (Player/Enemy/SlotMachines) not found. Check names and scene hierarchy.");
        }
    }
    

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTurn()
    {
        Debug.Log("Starting New Turn");
        combatLogMessage = "Rolling slots for new abilities...";

        if (CheckGameOver()) return;

        if (playerRoller != null){
            playerRoller.RollAbility();
        }
        if (enemyRoller != null){
            enemyRoller.RollAbility();
        }

        currentState = GameState.PlayerSlotDecision;
    }

    private void OnPlayerAbilityRolled(Ability rolledAbility)
    {
        playerRolledAbility = rolledAbility;
        combatLogMessage = $"You rolled the ability: {rolledAbility.abilityName}. Choose a slot or discard.";
        currentState = GameState.PlayerSlotDecision;
    }

    private void OnEnemyAbilityRolled(Ability rolledAbility)
    {
        enemyRolledAbility = rolledAbility;
        combatLogMessage += $"\nEnemy Rolled: {rolledAbility.abilityName}.";
    }

    public void EndSlotPhase()
    {
         if (currentState != GameState.PlayerSlotDecision) {
            return;
         }
        combatLogMessage = "Slot Phase complete. Select an ability from your 5 slots to use.";
        Debug.Log("Slot Phase complete. Moving on to Player Ability Selection Phase");
    }

    public void NextPhase()
    {
        switch (currentState)
        {
            case GameState.PlayerAbilitySelect:
                 if (enemyRolledAbility != null)
                {
                    combatLogMessage += $"\nEnemy uses: {enemyRolledAbility.abilityName}!";
                    // Enemy targets the player and passes the GameManager to log/control flow
                    enemyRolledAbility.Execute(enemy.gameObject, player.gameObject, this);
                }
                else
                {
                    combatLogMessage += "\nEnemy could not act (no rolled ability).";
                    // Immediately transition to the next turn if the enemy can't act
                    StartCoroutine(WaitAndStartTurn(1.5f)); 
                }
                break;
            case GameState.EnemyAction:
                StartCoroutine(WaitAndStartTurn(1.5f));
                break;
            default:
                Debug.LogWarning("Tried to advance phase from an invalid state: " + currentState);
                break;
        }
    }

    private IEnumerator WaitAndStartTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = GameState.SlotPhase;
        StartTurn();
    }

    private bool CheckGameOver()
    {
        if (player != null && player.Health.CurrentHealth <= 0)
        {
            currentState = GameState.GameOver;
            combatLogMessage = "Game Over! You Lost!";
            Debug.Log(combatLogMessage);
            return true;
        }
        if (enemy != null && enemy.Health.CurrentHealth <= 0)
        {
            currentState = GameState.GameOver;
            combatLogMessage = "Game Over! You Won!";
            Debug.Log(combatLogMessage);
            return true;
        }
        return false;
    }
}