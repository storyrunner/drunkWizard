using UnityEngine;
using System.Collections;
using System.Linq;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability/New Ability")]
public class Ability : ScriptableObject
{
    public enum AbilityType { Attack, Heal, Utility, SelfBuff }

    [Header("Basic Info")]
    public string abilityName = "New Ability";
    [TextArea(3, 5)]
    public string description = "A basic ability.";
    public AbilityType type = AbilityType.Attack;

    public Sprite abilityIcon;

    [Header("Combat Stats")]
    // General numeric value used for damage, healing, or utility power
    public int power = 10; 
    public float executionDelay = 0.5f;

    public IEnumerator Execute(GameObject user, GameObject target, GameManager gameManager)
    {
        // Get the Health components
        HealthComponent targetHealth = target.GetComponent<HealthComponent>();
        HealthComponent userHealth = user.GetComponent<HealthComponent>();

        if (targetHealth == null || userHealth == null)
        {
            Debug.LogError("Ability execution failed: Missing HealthComponent on user or target.");
            yield break; // Exit coroutine
        }

        yield return new WaitForSeconds(executionDelay);

        // --- Execute Effects ---
        switch (type)
        {
            case AbilityType.Attack:
                targetHealth.TakeDamage(power);
                gameManager.combatLogMessage += $"\n{abilityName} hit {targetHealth.gameObject.name} for {power} damage. {targetHealth.gameObject.name} Health: {targetHealth.CurrentHealth}";
                break;

            // ... [Retain other AbilityType cases (Heal, Utility, SelfBuff)] ...
            
            case AbilityType.Heal:
                // Heals the user
                userHealth.Heal(power);
                gameManager.combatLogMessage += $"\n{abilityName} healed {userHealth.gameObject.name} for {power} health. {userHealth.gameObject.name} Health: {userHealth.CurrentHealth}";
                break;
            
            case AbilityType.Utility:
                // Example: A weak attack that also heals the user (Drain Life)
                targetHealth.TakeDamage(power / 2);
                userHealth.Heal(power / 2);
                gameManager.combatLogMessage += $"\n{abilityName} drained {power / 2} health from {targetHealth.gameObject.name} and healed {userHealth.gameObject.name}.";
                break;
            
            case AbilityType.SelfBuff:
                // Example: Temporary stat change (Requires a more complex StatSystem component, but for now we'll use a minor heal/shield effect)
                userHealth.Heal(power);
                gameManager.combatLogMessage += $"\n{abilityName} shielded {userHealth.gameObject.name}, granting {power} effective health (Heal).";
                break;
        }

        // IMPORTANT: DO NOT call gameManager.NextPhase() here. The ExecuteTurnSequence coroutine
        // in the GameManager will automatically move to the next ability or phase.
    }
}