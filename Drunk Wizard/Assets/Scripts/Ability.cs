using UnityEngine;
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

    [Header("Combat Stats")]
    // General numeric value used for damage, healing, or utility power
    public int power = 10; 
    public float executionDelay = 0.5f;

    /// <summary>
    /// Executes the ability's effect.
    /// </summary>
    /// <param name="user">The GameObject using the ability (Player or Enemy).</param>
    /// <param name="target">The GameObject the ability is directed at (usually the opposite entity).</param>
    /// <param name="gameManager">Reference to the GameManager to handle flow control after execution.</param>
    public void Execute(GameObject user, GameObject target, GameManager gameManager)
    {
        // Get the Health components
        HealthComponent targetHealth = target.GetComponent<HealthComponent>();
        HealthComponent userHealth = user.GetComponent<HealthComponent>();

        if (targetHealth == null || userHealth == null)
        {
            Debug.LogError("Ability execution failed: Missing HealthComponent on user or target.");
            gameManager.NextPhase();
            return;
        }

        // Use a coroutine on the GameManager MonoBehaviour to handle the delay
        gameManager.StartCoroutine(ExecuteAfterDelay(targetHealth, userHealth, gameManager));
    }

    private System.Collections.IEnumerator ExecuteAfterDelay(HealthComponent targetHealth, HealthComponent userHealth, GameManager gameManager)
    {
        yield return new WaitForSeconds(executionDelay);

        switch (type)
        {
            case AbilityType.Attack:
                // Deals damage to the target
                targetHealth.TakeDamage(power);
                gameManager.combatLogMessage += $"\n{abilityName} hit {targetHealth.gameObject.name} for {power} damage. {targetHealth.gameObject.name} Health: {targetHealth.CurrentHealth}";
                break;

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

        // After execution is complete, tell the GameManager to advance the phase
        gameManager.NextPhase();
    }
}