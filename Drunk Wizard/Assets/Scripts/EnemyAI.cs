using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [Header("Configuration")]
    public int maxAbilitySlots = 3;
    
    [Header("References")]
    private Ability[] abilitySlots;
    private HealthComponent health;
    private PlayerController targetPlayer; // The player target

    // Simple priority list for AI decision-making
    public enum AIPriority { Attack, Defense, Utility }

    void Awake()
    {
        abilitySlots = new Ability[maxAbilitySlots];
        health = GetComponent<HealthComponent>();
        targetPlayer = FindFirstObjectByType<PlayerController>(); // Simple targeting
    }
    
    public HealthComponent Health => health;

    // --- Slot Phase Decision ---

    // Called by GameManager after the enemy slot machine rolls.
    public void HandleRolledAbility(Ability rolledAbility)
    {
        // 1. Check for empty slot
        for (int i = 0; i < maxAbilitySlots; i++)
        {
            if (abilitySlots[i] == null)
            {
                abilitySlots[i] = rolledAbility;
                Debug.Log($"Enemy AI saved '{rolledAbility.abilityName}' to empty slot {i + 1}.");
                return;
            }
        }

        // 2. If all slots are full, implement a simple replacement strategy:
        // Replace the oldest/weakest ability (or a random one for simplicity).
        
        // Strategy: Replace the lowest priority ability (e.g., replace utility/defense for a high attack)
        int lowestPriorityIndex = -1;
        float lowestValue = float.MaxValue; // Use a weighted score if needed
        
        // For simple example: Replace the utility ability, or the first one found.
        
        // Find the index of the first non-attack ability, or the first slot.
        for (int i = 0; i < maxAbilitySlots; i++)
        {
            if (abilitySlots[i].type == Ability.AbilityType.Utility)
            {
                lowestPriorityIndex = i;
                break;
            }
            if (abilitySlots[i].type == Ability.AbilityType.Defense)
            {
                lowestPriorityIndex = i;
            }
            if (lowestPriorityIndex == -1) // If still nothing, just pick the first one (0)
            {
                lowestPriorityIndex = 0;
            }
        }

        if (lowestPriorityIndex != -1)
        {
            Debug.Log($"Enemy AI replaced '{abilitySlots[lowestPriorityIndex].abilityName}' with '{rolledAbility.abilityName}'.");
            abilitySlots[lowestPriorityIndex] = rolledAbility;
        }
        else
        {
            // Should not happen if slots are full, but good practice.
            Debug.Log("Enemy AI decided to discard the rolled ability.");
        }
    }
    
    // --- Action Phase Decision ---
    
    // Called by GameManager during the Enemy Action Phase.
    public void ExecuteAction(GameManager gameManager)
    {
        Ability abilityToUse = SelectBestAbility();

        if (abilityToUse != null)
        {
            // 1. Find the index of the chosen ability
            int usedIndex = System.Array.IndexOf(abilitySlots, abilityToUse);
            
            // 2. Execute the ability
            abilityToUse.Execute(this.gameObject, targetPlayer.gameObject);
            
            // 3. Clear the slot
            abilitySlots[usedIndex] = null;
        }
        else
        {
            Debug.Log("Enemy has no abilities to use this turn. Skipping action.");
        }

        // 4. Notify the GameManager to advance to the End Turn phase
        gameManager.NextPhase();
    }

    // AI Logic for choosing which ability to use.
    private Ability SelectBestAbility()
    {
        // Simple AI Strategy:
        // 1. Always prioritize Attack abilities.
        // 2. If player health is low, use attack.
        // 3. If enemy health is low, prioritize healing/defense.

        // Filter out null abilities
        var activeAbilities = abilitySlots.Where(a => a != null).ToList();

        if (activeAbilities.Count == 0) return null;

        // --- Priority 1: High Attack ---
        Ability bestAttack = activeAbilities
            .Where(a => a.type == Ability.AbilityType.Attack)
            .OrderByDescending(a => a.attackPower)
            .FirstOrDefault();
        
        if (bestAttack != null) return bestAttack;
        
        // --- Priority 2: Low Health Defense/Heal ---
        if (health.CurrentHealth <= health.MaxHealth / 3)
        {
            Ability bestHeal = activeAbilities
                .Where(a => a.isHealing || a.type == Ability.AbilityType.Defense)
                .OrderByDescending(a => a.defenseBoost)
                .FirstOrDefault();
            if (bestHeal != null) return bestHeal;
        }

        // --- Priority 3: Use anything else (Utility or left-over Defense) ---
        return activeAbilities.FirstOrDefault();
    }
}
