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
    private EnemyAI targetEnemy; 

    private List<Ability> selectedAbilityChain = new List<Ability>();

    // =========================================================
    // NEW FIELDS FOR "SELECT AND SWAP" MECHANIC
    // =========================================================
    private Ability abilityToSwap = null;
    private int rolledIndexToConsume = -1; 
    // =========================================================


    void Awake()
    {
        abilitySlots = new Ability[maxAbilitySlots];
        health = GetComponent<HealthComponent>();
        
        gameManager = FindFirstObjectByType<GameManager>();
        targetEnemy = FindFirstObjectByType<EnemyAI>();
    }

    public HealthComponent Health => health;

    // --- Slot Management (The core Player mechanic) ---

    // NEW: PUBLIC GETTER to fix the CS0122 error
    public Ability GetAbilityToSwap()
    {
        return abilityToSwap;
    }

    // =========================================================
    // SETTER METHOD (Called by RolledAbilityUI when the roll is clicked)
    // =========================================================
    public void SetAbilityToSwap(Ability rolledAbility, int rolledIndex)
    {
        if (abilityToSwap == rolledAbility)
        {
            abilityToSwap = null;
            rolledIndexToConsume = -1;
            Debug.Log($"Ability un-selected: {rolledAbility.abilityName}");
        }
        else
        {
            abilityToSwap = rolledAbility;
            rolledIndexToConsume = rolledIndex;
            Debug.Log($"Ability to swap set: {rolledAbility.abilityName} from rolled index {rolledIndex}");
        }
    }

    // GETTER METHOD (Used by PlayerSlotUI to enable its buttons)
    public bool IsAbilityToSwapSet()
    {
        return abilityToSwap != null;
    }

    // FINALIZER METHOD (Called by PlayerSlotUI when the permanent slot is clicked)
    public void FinalizeRolledSwap(int destinationSlotIndex)
    {
        if (abilityToSwap == null || rolledIndexToConsume == -1) 
        {
            Debug.LogError("Attempted to finalize swap without an ability selected!");
            return;
        }

        // 1. Perform the save (overwrite the permanent slot)
        if (destinationSlotIndex >= 0 && destinationSlotIndex < maxAbilitySlots)
        {
            if (abilitySlots[destinationSlotIndex] != null)
            {
                Debug.Log($"Replaced old ability '{abilitySlots[destinationSlotIndex].abilityName}' in slot {destinationSlotIndex + 1}.");
            }
            abilitySlots[destinationSlotIndex] = abilityToSwap;
        }

        // 2. Consume the ability from the rolled list
        if (gameManager.playerRolledAbilities.Count > rolledIndexToConsume)
        {
            gameManager.playerRolledAbilities.RemoveAt(rolledIndexToConsume);
        }

        // 3. Clear the temporary swap state
        Debug.Log($"Swap completed. Remaining rolls: {gameManager.playerRolledAbilities.Count}");
        abilityToSwap = null;
        rolledIndexToConsume = -1;

        if (gameManager.playerRolledAbilities.Count == 0)
    {
        Debug.Log("All rolled abilities consumed. Advancing phase.");
        
        // This method should call gameManager.SetGameState(GameManager.GameState.PlayerAbilitySelect)
        FinalizeSlotDecision(); 
    }
    }
    // =========================================================
    
    // METHOD MODIFIED TO USE SetGameState() to fix CS0200
    public void FinalizeSlotDecision()
    {
        abilityToSwap = null;
        rolledIndexToConsume = -1;
        
        // FIX: Use the new public setter method on GameManager
        gameManager.SetGameState(GameManager.GameState.PlayerAbilitySelect);
        gameManager.combatLogMessage += "\nSlot decision finalized. Choose your attack chain.";
    }

    // --- Ability Chain Logic (Unchanged) ---
    
    public void SelectAbilityForChain(int slotIndex)
    {
        if (gameManager.CurrentState != GameManager.GameState.PlayerAbilitySelect) return;
        
        if (abilitySlots[slotIndex] != null)
        {
            Ability selectedAbility = abilitySlots[slotIndex];
            selectedAbilityChain.Add(selectedAbility);
            gameManager.combatLogMessage += $"\nChain: {string.Join(", ", selectedAbilityChain.Select(a => a.abilityName))}";

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
            gameManager.StartCombatSequence(selectedAbilityChain);
        }
        else
        {
            gameManager.StartCombatSequence(new List<Ability>()); 
        }

        selectedAbilityChain.Clear(); 
    }
    
    public bool IsSlotEmpty(int index)
    {
        return abilitySlots[index] == null;
    }

    public Ability GetAbilityInSlot(int index)
    {
        return abilitySlots[index];
    }

    public List<Ability> GetSelectedAbilityChain()
    {
        return selectedAbilityChain;
    }
}