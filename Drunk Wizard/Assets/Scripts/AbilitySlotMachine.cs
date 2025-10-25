using UnityEngine;
using System.Collections.Generic;

public class AbilitySlotMachine : MonoBehaviour
{
    public List<Ability> availableAbilities;

    // This event notifies the GameManager and UI when a roll is complete.
    public delegate void AbilityRolledHandler(Ability rolledAbility);
    public event AbilityRolledHandler OnAbilityRolled;

    void Start()
    {
        if (availableAbilities == null || availableAbilities.Count == 0)
        {
            Debug.LogError($"{gameObject.name} Slot Machine has no abilities assigned!");
        }
    }

    // Simple randomization to simulate the slot machine roll.
    public void RollAbility()
    {
        if (availableAbilities.Count == 0)
        {
            Debug.LogError("Slot Machine has no abilities to roll!");
        }

        // Simulate rolling by picking a random ability
        int randomIndex = Random.Range(0, availableAbilities.Count);
        Ability rolledAbility = availableAbilities[randomIndex];

        Debug.Log($"Slot Machine rolled: {rolledAbility.abilityName}");

        // Broadcast the result
        if (OnAbilityRolled != null)
        {
            OnAbilityRolled(rolledAbility);
        }

    }
}
