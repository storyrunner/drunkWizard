using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RolledAbilityUI : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The index of this roll (0, 1, or 2)")]
    public int rolledAbilityIndex; 

    [Header("References")]
    public GameObject rolledPanel; 
    public TextMeshProUGUI abilityNameText;
    public Image abilityIconImage;

    [Header("Select Button")]
    // The single button the player clicks to initiate the swap
    public Button selectRollButton; 
    
    private GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerController = FindFirstObjectByType<PlayerController>();
        
        if (selectRollButton != null)
        {
            // Listener for the new single button
            selectRollButton.onClick.AddListener(OnRolledAbilitySelected);
        }
        
        rolledPanel.SetActive(false); 
    }

    void Update()
    {
        // 1. Check if the player is in the decision phase
        bool isDecisionPhase = gameManager.CurrentState == GameManager.GameState.PlayerSlotDecision;

        // 2. Check if an ability is available at this UI's assigned index (0, 1, or 2)
        bool isAbilityAvailable = gameManager.playerRolledAbilities.Count > rolledAbilityIndex; 

        // 3. Update Display
        if (isDecisionPhase && isAbilityAvailable)
        {
            Ability currentRolledAbility = gameManager.playerRolledAbilities[rolledAbilityIndex];
            abilityNameText.text = currentRolledAbility.abilityName;
            
            abilityIconImage.sprite = currentRolledAbility.abilityIcon;
            abilityIconImage.enabled = true;
            
            rolledPanel.SetActive(true);
        }
        else
        {
            rolledPanel.SetActive(false);
            abilityIconImage.enabled = false; 
        }

        // 4. Update Button Interactivity
        
        bool isCurrentlySelected = false;
        if (isAbilityAvailable && playerController.IsAbilityToSwapSet())
        {
            // Use the public getter (assuming you added GetAbilityToSwap() to PlayerController)
            if (playerController.GetAbilityToSwap() == gameManager.playerRolledAbilities[rolledAbilityIndex])
            {
                isCurrentlySelected = true;
            }
        }
        
        // Prevent clicking other rolls if one is already selected (unless this one is currently selected to un-select it)
        bool isHoldingDifferentSwap = playerController.IsAbilityToSwapSet() && !isCurrentlySelected;

        if (selectRollButton != null)
        {
            selectRollButton.interactable = isDecisionPhase && isAbilityAvailable && !isHoldingDifferentSwap;
            
            // Optional: Change button color/sprite when selected
            if (isCurrentlySelected)
            {
                selectRollButton.image.color = Color.green;
            }
            else
            {
                selectRollButton.image.color = Color.white;
            }
        }
    }
    
    // Called when the Rolled Ability's single button is clicked
    private void OnRolledAbilitySelected()
    {
        if (gameManager.playerRolledAbilities.Count > rolledAbilityIndex)
        {
            Ability ability = gameManager.playerRolledAbilities[rolledAbilityIndex];
            
            // This method handles selecting or un-selecting the roll
            playerController.SetAbilityToSwap(ability, rolledAbilityIndex);
        }
    }
}