using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public int maxAbilitySlots = 5;
    
    [Header("References")]
    private Ability[] abilitySlots;
    private HealthComponent health;
    private GameManager gameManager;
    private EnemyAI targetEnemy; // The enemy target

    private List<Ability> selectedAbilityChain = new List<Ability>();

    void Awake()
    {
        abilitySlots = new Ability[maxAbilitySlots];
        health = GetComponent<HealthComponent>();
        
        // Find necessary components
        gameManager = FindFirstObjectByType<GameManager>();
        targetEnemy = FindFirstObjectByType<EnemyAI>(); // Simple targeting for now
    }

    public HealthComponent Health => health;

    // --- Slot Management (The core Player mechanic) ---

    // Public method called by a UI button after the slot machine roll.
    // Index is 0-4, corresponding to which slot the player chooses to overwrite.
    public void SaveRolledAbility(Ability rolledAbility, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < maxAbilitySlots)
        {
            if (abilitySlots[slotIndex] != null)
            {
                Debug.Log($"Replaced old ability '{abilitySlots[slotIndex].abilityName}' in slot {slotIndex + 1}.");
            }
            abilitySlots[slotIndex] = rolledAbility;
            Debug.Log($"Saved new ability '{rolledAbility.abilityName}' to slot {slotIndex + 1}.");
        }
        else
        {
            Debug.LogError("Invalid slot index chosen.");
        }
    }

    public void FinalizeSlotDecision()
    {
        gameManager.EndSlotPhase(); 
    }

    public void SelectAbilityForChain(int slotIndex)
    {
        if (gameManager.CurrentState != GameManager.GameState.PlayerAbilitySelect) return;
        
        Ability abilityToSelect = abilitySlots[slotIndex];

        if (abilityToSelect != null)
        {
            selectedAbilityChain.Add(abilityToSelect);
            Debug.Log($"Added '{abilityToSelect.abilityName}' to the chain. Chain Length: {selectedAbilityChain.Count}");
            gameManager.combatLogMessage = $"Chain: {string.Join(", ", selectedAbilityChain.Select(a => a.abilityName))}";

            // The ability is consumed upon execution, so we clear the slot now.
            // This prevents using the same ability twice in one turn.
            abilitySlots[slotIndex] = null; 
        }
        else
        {
            Debug.LogWarning($"Slot {slotIndex + 1} is empty. Cannot select.");
        }
    }
    
    public void ConfirmAbilityChain()
    {
        if (gameManager.CurrentState != GameManager.GameState.PlayerAbilitySelect) return;

        if (selectedAbilityChain.Count > 0)
        {
            // Pass the chain to the GameManager and begin combat sequence
            gameManager.StartCombatSequence(selectedAbilityChain);
        }
        else
        {
            // If the player selects nothing, they pass the turn.
            gameManager.StartCombatSequence(new List<Ability>()); 
        }

        // The chain list is cleared by the GameManager after execution.
        selectedAbilityChain.Clear(); 
    }
    
    // Utility function to check if a slot is empty (useful for UI)
    public bool IsSlotEmpty(int index)
    {
        return abilitySlots[index] == null;
    }

    // Returns the current ability in a slot (useful for UI display)
    public Ability GetAbilityInSlot(int index)
    {
        return abilitySlots[index];
    }
}