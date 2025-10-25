using UnityEngine;
using UnityEngine.Accessibility;
using System.Linq;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRoller = GameObject.Find("PlayerSlotMachine").GetComponent<AbilitySlotMachine>();
        enemyRoller = GameObject.Find("EnemySlotMachine").GetComponent<AbilitySlotMachine>();
        player = FindFirstObjectByType<PlayerController>();
        enemy = FindFirstObjectByType<EnemyAI>();

        if (playerRoller != null) playerRoller.OnAbilityRolled += OnPlayerAbilityRolled;
        if (enemyRoller != null) enemyRoller.OnAbilityRolled += OnEnemyAbilityRolled;

        currentState = GameState.SlotPhase;
        StartTurn();
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

        playerRoller.RollAbility();
        enemyRoller.RollAbility();

        currentState = GameState.PlayerSlotDecision;
    }

    private void OnPlayerAbilityRolled(Ability rolledAbility)
    {
        playerRolledAbility = rolledAbility;
        combatLogMessage = $"You rolled the ability: {rolledAbility.abilityName}. Choose a slot or discard.";
    }

    private void OnEnemyAbilityRolled(Ability rolledAbility)
    {
        enemyRolledAbility = rolledAbility;
        enemy.HandleRolledAbility(enemyRolledAbility);
    }

    public void EndSlotPhase()
    {
        currentState = GameState.PlayerAbilitySelect;
        combatLogMessage = "Slot Phase complete. Select an ability from your 5 slots to use.";
        Debug.Log("Slot Phase complete. Moving on to Player Ability Selection Phase");
    }

    public void NextPhase()
    {
        switch (currentState)
        {
            case GameState.PlayerAbilitySelect:
                currentState = GameState.EnemyAction;
                combatLogMessage = "Enemy is executing its action...";
                Debug.Log("Enemy Action Phase");
                enemy.ExecuteAction(this);
                break;
            case GameState.EnemyAction:
                if (CheckGameOver())
                {
                    currentState = GameState.GameOver;
                    combatLogMessage = "Game Over!";
                    Debug.Log("Game Over!");
                }
                else
                {
                    StartTurn();
                }
                break;
            default:
                Debug.LogWarning("Tried to advance phase from an invalid state: " + currentState);
                break;
        }
    }

    private bool CheckGameOver()
    {
        if (player.Health.IsDead)
        {
            Debug.Log("You Lost!");
            combatLogMessage = "You lost! The game is over.";
            return true;
        }
        if (enemy.Health.IsDead)
        {
            Debug.Log("You Won!");
            combatLogMessage = "You won! The game is over.";
            return true;
        }
        return false;
    }

}