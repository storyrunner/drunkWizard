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
    // The single button that handles both selection and saving
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
        
        // Listener calls the dual-purpose handler
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnDestinationSlotClicked);
    }

    void Update()
    {
        UpdateSlotDisplay();
        
        bool isDecisionPhase = gameManager.CurrentState == GameManager.GameState.PlayerSlotDecision;
        bool isSelectPhase = gameManager.CurrentState == GameManager.GameState.PlayerAbilitySelect;
        bool isSlotNotEmpty = !playerController.IsSlotEmpty(slotIndex);
        
        // Check if PlayerController is holding a rolled ability for a swap
        bool isHoldingSwap = playerController.IsAbilityToSwapSet();

        // The button is interactable IF:
        // 1. (Select Phase AND Slot has an ability) -OR-
        // 2. (Decision Phase AND Player is holding a roll to swap)
        selectButton.interactable = (isSelectPhase && isSlotNotEmpty) || (isDecisionPhase && isHoldingSwap);
        
        // --- Visual Feedback ---
        if (isDecisionPhase && isHoldingSwap)
        {
            // Highlight this slot to show it's a valid swap destination
            backgroundImage.color = Color.yellow; 
        }
        else if (isSelectPhase && isSlotNotEmpty)
        {
            // Standard selection color when building the chain
            backgroundImage.color = Color.white; 
        }
        else
        {
            // Default color for inactive/empty slot
            backgroundImage.color = Color.grey; 
        }
    }

    // Dual-purpose click handler
    private void OnDestinationSlotClicked()
    {
        // 1. SWAP LOGIC: Executed if the player is holding a roll AND it's the decision phase
        if (playerController.IsAbilityToSwapSet() && gameManager.CurrentState == GameManager.GameState.PlayerSlotDecision)
        {
            playerController.FinalizeRolledSwap(slotIndex);
        }
        // 2. SELECTION LOGIC: Executed if it's the ability selection phase
        else if (gameManager.CurrentState == GameManager.GameState.PlayerAbilitySelect)
        {
            playerController.SelectAbilityForChain(slotIndex);
        }
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
}