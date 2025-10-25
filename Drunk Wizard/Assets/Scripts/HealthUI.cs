using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    public HealthComponent healthComponent;
    public TextMeshProUGUI healthText; 
    public Image healthBarFill; 

    private void Start()
    {
        if (healthComponent == null)
        {
            Debug.LogError("HealthComponent reference is missing in HealthUI.");
            enabled = false;
        }
    }

    // Using Update() for simple continuous UI updates.
    void Update()
    {
        if (healthComponent != null)
        {
            UpdateHealthDisplay();
        }
    }

    private void UpdateHealthDisplay()
    {
        int current = healthComponent.CurrentHealth;
        int max = healthComponent.MaxHealth;

        // Update the fill amount of the health bar
        float fillAmount = (float)current / max;
        healthBarFill.fillAmount = fillAmount;

        // Update the text display
        if (healthText != null)
        {
            healthText.text = $"{current} / {max}";
        }
    }
}