using UnityEngine;
using System.Collections.Generic;

public class AbilitySlotMachine : MonoBehaviour
{
    public List<Ability> availableAbilities;

    // Modified event to broadcast a list of rolled abilities
    public delegate void AbilitiesRolledHandler(List<Ability> rolledAbilities);
    public event AbilitiesRolledHandler OnAbilitiesRolled; // Changed event name and signature

    // Constant for how many abilities to roll
    private const int RollsPerTurn = 3;

    void Start()
    {
        if (availableAbilities == null || availableAbilities.Count == 0)
        {
            Debug.LogError($"{gameObject.name} Slot Machine has no abilities assigned!");
        }
    }

    // Now rolls the specified number of abilities.
    public void RollAbilities()
    {
        if (availableAbilities.Count == 0)
        {
            Debug.LogError("Slot Machine has no abilities to roll!");
            return;
        }

        List<Ability> rolledAbilities = new List<Ability>();

        for (int i = 0; i < RollsPerTurn; i++)
        {
            // Simulate rolling by picking a random ability
            int randomIndex = Random.Range(0, availableAbilities.Count);
            Ability rolledAbility = availableAbilities[randomIndex];
            rolledAbilities.Add(rolledAbility);

            Debug.Log($"Reel {i + 1} rolled: {rolledAbility.abilityName}");
        }

        // Broadcast the result (using the modified event)
        if (OnAbilitiesRolled != null)
        {
            OnAbilitiesRolled(rolledAbilities);
        }
    }
}
