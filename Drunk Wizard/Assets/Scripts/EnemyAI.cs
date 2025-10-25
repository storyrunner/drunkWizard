using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    private HealthComponent health;
    private PlayerController targetPlayer; // The player target

    void Awake()
    {
        // Find the player using the non-deprecated method
        targetPlayer = FindFirstObjectByType<PlayerController>(); 
        
        // Get the Health component attached to this GameObject
        health = GetComponent<HealthComponent>();
    }
    
    // Public getter for the Health Component, used by GameManager and Ability.cs
    public HealthComponent Health => health;

    // --- Action Phase Decision ---
    
    // NOTE: The enemy's action is now simplified. 
    // It is called by the GameManager, which passes the rolled ability directly.
    
    /// <summary>
    /// Placeholder method to execute the Enemy's action for the turn. 
    /// In this simplified slotless AI, the GameManager calls the ability's Execute method directly.
    /// This method can be expanded later if you want the enemy to choose from a different pool of actions.
    /// </summary>
    /// <param name="gameManager">Reference to the GameManager for flow control.</param>
    public void ExecuteAction(GameManager gameManager)
    {
        // For the slotless design, the GameManager handles calling the rolled ability's Execute method.
        // We only need to call NextPhase here if the GameManager failed to execute the ability.
        
        // Since GameManager handles the ability execution and then calls NextPhase, 
        // this method primarily serves as a placeholder for more complex future AI logic.

        // In the current implementation, this method is actually redundant because 
        // the GameManager handles all the execution:
        /*
        case GameState.PlayerAbilitySelect:
            // ...
            enemyRolledAbility.Execute(enemy.gameObject, player.gameObject, this);
        */
        
        // For safety and future-proofing, we will still allow the GameManager to proceed:
        // However, we rely on the Ability.Execute() coroutine to call gameManager.NextPhase()
        
        // For now, if this were called, it would simply inform the GameManager it's done.
        // The actual ability execution is already handled in GameManager.NextPhase()
        
        // We will do nothing here to avoid double-calling NextPhase, 
        // relying on the delay inside Ability.Execute() to advance the game.
        
        Debug.Log("Enemy AI is ready for the next phase (Ability execution is already triggered).");
    }
}
