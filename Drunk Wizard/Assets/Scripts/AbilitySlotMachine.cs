using UnityEngine;
using System.Collections.Generic;

public class AbilitySlotMachine : MonoBehaviour
{
    public List<Ability> availableAbilities = new List<Ability>();

    // This event notifies the GameManager and UI when a roll is complete.
    public event System.Action<Ability> OnAbilityRolled;

    // Simple randomization to simulate the slot machine roll.
    public Ability RollAbility()
    {
        if (availableAbilities.Count == 0)
        {
            Debug.LogError("Slot Machine has no abilities to roll!");
            return null;
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

        return rolledAbility;
    }
}
