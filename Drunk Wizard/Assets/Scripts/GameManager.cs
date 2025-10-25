using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        AwaitStart,
        SlotPhase,
        PlayerSlotDecision,
        PlayerAbilitySelect, // Player chooses ability chain
        CombatSequence,      // Abilities are executing sequentially
        GameOver
    }

    private GameState currentState = GameState.AwaitStart;

    public GameState CurrentState => currentState;

    public AbilitySlotMachine playerRoller;
    public AbilitySlotMachine enemyRoller;
    public PlayerController player; 
    public EnemyAI enemy; 

    public List<Ability> playerRolledAbilities = new List<Ability>();
    public List<Ability> enemyRolledAbilities = new List<Ability>();

    private List<Ability> playerAbilityChain = new List<Ability>();

    public string combatLogMessage = "";



    void Awake()
    {
        playerRoller.RollAbilities();

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
        if (playerRoller != null) playerRoller.OnAbilitiesRolled += OnPlayerAbilitiesRolled;
        if (enemyRoller != null) enemyRoller.OnAbilitiesRolled += OnEnemyAbilitiesRolled;
        
        // Final sanity check
        if (playerRoller == null || enemyRoller == null || player == null || enemy == null)
        {
            Debug.LogError("FATAL ERROR: Core game components (Player/Enemy/SlotMachines) not found. Check names and scene hierarchy.");
        }
    }
    
    public void StartGame()
    {
        currentState = GameState.SlotPhase;
        // The GameManager is now responsible for enabling/disabling the main UI vs Start/Pause/Game Over screens
        // UIController.Instance.ShowGameUI(); // Example of UI control
        StartTurn();
    }

    public void StartTurn()
    {
        Debug.Log("Starting New Turn");
        combatLogMessage = "Rolling slots for new abilities...";

        if (CheckGameOver()) return;

        if (playerRoller != null){
            playerRoller.RollAbilities(); 
        }
        if (enemyRoller != null){
            enemyRoller.RollAbilities(); 
        }

        currentState = GameState.PlayerSlotDecision;
    }

    private void OnPlayerAbilitiesRolled(List<Ability> rolledAbilities)
    {
        playerRolledAbilities = rolledAbilities;
        // Inform the player of their choices
        string abilityNames = string.Join(", ", rolledAbilities.Select(a => a.abilityName));
        combatLogMessage = $"You rolled the abilities: {abilityNames}. Choose a slot for each, or discard them.";
        currentState = GameState.PlayerSlotDecision;
    }

    private void OnEnemyAbilitiesRolled(List<Ability> rolledAbilities)
    {
        enemyRolledAbilities = rolledAbilities;
        combatLogMessage += $"\nEnemy Rolled: {enemyRolledAbilities.First().abilityName} and 2 others.";
    }

    public void EndSlotPhase()
    {
         if (currentState != GameState.PlayerSlotDecision) {
            return;
         }
        playerRolledAbilities.Clear();
        currentState = GameState.PlayerAbilitySelect;
        combatLogMessage = "Slot Phase complete. Select an ability from your 5 slots to use.";
        Debug.Log("Slot Phase complete. Moving on to Player Ability Selection Phase");
    }

    public void StartCombatSequence(List<Ability> chain)
    {
        if (currentState != GameState.PlayerAbilitySelect) return;

        playerAbilityChain = chain;
        currentState = GameState.CombatSequence;
        
        // Start the sequential execution coroutine
        StartCoroutine(ExecuteTurnSequence());
    }

    private IEnumerator ExecuteTurnSequence()
    {
        if (playerAbilityChain.Count > 0)
        {
            combatLogMessage = "Player turn: Executing ability chain...";
            foreach (Ability ability in playerAbilityChain)
            {
                if (CheckGameOver()) yield break;
                
                combatLogMessage += $"\nPlayer uses: {ability.abilityName}!";
                // Execute the ability and WAIT for it to finish.
                yield return StartCoroutine(ability.Execute(player.gameObject, enemy.gameObject, this));
            }
            playerAbilityChain.Clear();
        }
        else
        {
             combatLogMessage += "\nPlayer passed their turn.";
             yield return new WaitForSeconds(1f);
        }
        
        // 2. --- Enemy's Ability Chain (all 3 rolls) ---
        if (enemyRolledAbilities.Count > 0)
        {
            combatLogMessage += "\n\nEnemy turn: Executing all rolled abilities...";
            foreach (Ability ability in enemyRolledAbilities)
            {
                if (CheckGameOver()) yield break;
                
                combatLogMessage += $"\nEnemy uses: {ability.abilityName}!";
                // Execute the ability and WAIT for it to finish.
                yield return StartCoroutine(ability.Execute(enemy.gameObject, player.gameObject, this));
            }
            enemyRolledAbilities.Clear(); // Enemy rolls are always consumed
        }
        else
        {
            combatLogMessage += "\nEnemy had no abilities to use.";
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(WaitAndStartTurn(1.5f));
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