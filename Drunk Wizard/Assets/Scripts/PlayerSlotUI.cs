using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlotUI : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The index of this slot (0-4)")]
    public int slotIndex; 

    [Header("References")]
    public TextMeshProUGUI abilityNameText;
    public Button selectButton; 
    public Image backgroundImage; 
    public Image abilityIconImage;

    private PlayerController playerController;
    private GameManager gameManager;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        gameManager = FindFirstObjectByType<GameManager>();

        if (playerController == null || gameManager == null)
        {
            Debug.LogError("Missing core component in PlayerSlotUI.");
            enabled = false;
            return;
        }
        
        // Add listener for the main Select button
        // This is the new chain selection action
        selectButton.onClick.AddListener(() => playerController.SelectAbilityForChain(slotIndex));
    }

    void Update()
    {
        UpdateSlotDisplay();
        
        // Only enable selection button during the PlayerAbilitySelect phase
        bool isSelectPhase = gameManager.CurrentState == GameManager.GameState.PlayerAbilitySelect;
        bool isSlotNotEmpty = !playerController.IsSlotEmpty(slotIndex);
        
        selectButton.interactable = isSelectPhase && isSlotNotEmpty;
        
        // Optional visual feedback
        backgroundImage.color = selectButton.interactable ? Color.white : Color.grey;
        UpdateSlotDisplay();
    }

    public void UpdateSlotDisplay()
    {
        Ability ability = playerController.GetAbilityInSlot(slotIndex);
        if (ability != null)
        {
            abilityNameText.text = ability.abilityName;
            abilityIconImage.sprite = ability.abilityIcon;
            abilityIconImage.enabled = true;
        }
        else
        {
            abilityNameText.text = $"Slot {slotIndex + 1}: EMPTY";
            abilityIconImage.enabled = false;
        }
    }

    private void OnSelectAbilityClicked()
    {
        // --- [Rest of the OnSelectAbilityClicked logic] ---
        // This method simply calls the logic in PlayerController
        playerController.SelectAbilityForChain(slotIndex);
    }
}