using UnityEngine;

// This defines the structure for all abilities in the game.
[CreateAssetMenu(fileName = "NewAbility", menuName = "Game/Ability")]
public class Ability : ScriptableObject
{
    [Header("Ability Info")]
    public string abilityName = "New Ability";
    [TextArea(3, 5)]
    public string description = "What this ability does.";

    [Header("Combat Stats")]
    // Example fields - customize these based on your game's complexity
    public int attackPower = 0;
    public int defenseBoost = 0;
    public bool isHealing = false;

    // Defines what the ability is generally used for
    public enum AbilityType { Attack, Defense, Utility }
    public AbilityType type;

    public void Execute(GameObject user, GameObject target)
    {
        Debug.Log($"{user.name} uses {abilityName} on {target.name}!");

        // Handle Attack Logic
        if (attackPower > 0)
        {
            HealthComponent targetHealth = target.GetComponent<HealthComponent>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackPower);
                // Optional: user's health component could apply a self-buff or de-buff here
            }
        }
        
        // Handle Defense/Healing/Utility Logic
        if (defenseBoost > 0)
        {
            HealthComponent userHealth = user.GetComponent<HealthComponent>();
            if (userHealth != null)
            {
                // Simple logic: If it's a "Defense" type, heal the user.
                if (type == AbilityType.Defense)
                {
                    userHealth.Heal(defenseBoost);
                }
                // You could add complex logic here (e.g., if type is 'Utility', grant a temporary shield).
            }
        }
    }
}
