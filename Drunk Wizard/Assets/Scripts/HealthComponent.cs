using UnityEngine;

// A simple component to manage health, damage, and healing.
public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Reduces health, checks for death, and notifies the GameManager (if necessary).
    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Health remaining: {currentHealth}");

        if (IsDead)
        {
            Debug.Log($"{gameObject.name} has been defeated!");
            // Notify GameManager that this entity is dead.
            // FindFirstObjectByType<GameManager>().CheckGameOver(); 
        }
    }

    // Increases health, clamped by maxHealth.
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        Debug.Log($"{gameObject.name} healed for {healAmount}. Current Health: {currentHealth}");
    }
}