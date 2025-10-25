using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public int maxAbilitySlots = 5;
    
    [Header("References")]
    private Ability[] abilitySlots;
    private HealthComponent health;
    private GameManager gameManager;
    private EnemyAI targetEnemy; // The enemy target

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
            
            // Crucial: After saving, notify the GameManager to proceed to the next phase.
            gameManager.EndSlotPhase(); 
        }
        else
        {
            Debug.LogError("Invalid slot index chosen.");
        }
    }

    // Public method called by the 'Discard' UI button.
    public void DiscardRolledAbility()
    {
        Debug.Log("Player discarded the rolled ability.");
        gameManager.EndSlotPhase();
    }
    
    // Public method called by a UI button during the Player Ability Select phase.
    public void UseAbility(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < maxAbilitySlots && abilitySlots[slotIndex] != null)
        {
            Ability abilityToUse = abilitySlots[slotIndex];
            
            // 1. Execute the ability
            abilityToUse.Execute(this.gameObject, targetEnemy.gameObject, gameManager);

            // 2. Clear the used slot
            abilitySlots[slotIndex] = null;
            Debug.Log($"Used and cleared slot {slotIndex + 1}.");

            // 3. Notify the GameManager to advance to the Enemy Action Phase
            gameManager.NextPhase();
        }
        else
        {
            Debug.LogWarning($"Cannot use ability: Slot {slotIndex + 1} is empty or invalid.");
        }
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