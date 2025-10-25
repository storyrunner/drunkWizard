using UnityEngine;
using System.Collections;

// A simple component to manage health, damage, and healing.
public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    public SpriteRenderer characterRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    private Color originalColor;

    void Awake()
    {
        currentHealth = maxHealth;

        if (characterRenderer != null)
        {
            originalColor = characterRenderer.color;
        }
    }

    // Reduces health, checks for death, and notifies the GameManager (if necessary).
    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Health remaining: {currentHealth}");

        if (characterRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }

        if (IsDead)
        {
            // NEW: Hide character on death
            Debug.Log($"{gameObject.name} has been defeated!");
            if (characterRenderer != null) characterRenderer.enabled = false;
            // Optionally: Trigger a death animation/particle effect here
        }
        // The GameManager's ExecuteTurnSequence() checks IsDead after *every* ability execution, 
            // so we do not need to call CheckGameOver() here.
    }

    private IEnumerator DamageFlash()
    {
        characterRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        characterRenderer.color = originalColor;
    }

    // Increases health, clamped by maxHealth.
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        Debug.Log($"{gameObject.name} healed for {healAmount}. Current Health: {currentHealth}");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        // Ensure the character is visible upon reset
        if (characterRenderer != null)
        {
            characterRenderer.enabled = true;
            characterRenderer.color = originalColor; // Also reset the color
        }
        Debug.Log($"{gameObject.name} health reset to {maxHealth}.");
    }
}