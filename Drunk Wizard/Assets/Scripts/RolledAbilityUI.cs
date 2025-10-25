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

    [Header("Swap Buttons")]
    // The 5 buttons for the player to swap the rolled ability into a permanent slot
    public Button[] swapIntoSlotButtons = new Button[5]; 
    
    private GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerController = FindFirstObjectByType<PlayerController>();
        
        // Set up the listeners for the 5 swap buttons
        for (int i = 0; i < swapIntoSlotButtons.Length; i++)
        {
            int slotIndex = i;
            swapIntoSlotButtons[i].onClick.AddListener(() => OnSwapToSlotClicked(slotIndex));
        }

        rolledPanel.SetActive(false); 
    }

    void Update()
    {
        // 1. Check if the player is in the decision phase
        bool isDecisionPhase = gameManager.CurrentState == GameManager.GameState.PlayerSlotDecision;

        // 2. Check if this specific rolled ability is still available in the list
        // Note: The list shifts when an ability is saved, so we check against the list's *current* contents.
        bool isAbilityAvailable = isDecisionPhase && gameManager.playerRolledAbilities.Count > rolledAbilityIndex;

        // 3. Update Display
        if (isAbilityAvailable)
        {
            Ability currentRolledAbility = gameManager.playerRolledAbilities[rolledAbilityIndex];
            abilityNameText.text = currentRolledAbility.abilityName;
            
            // NEW: Set the icon and make the image visible
            abilityIconImage.sprite = currentRolledAbility.abilityIcon;
            abilityIconImage.enabled = true;
            
            rolledPanel.SetActive(true);
        }
        else
        {
            rolledPanel.SetActive(false);
            // NEW: Ensure the image is disabled when not active
            abilityIconImage.enabled = false; 
        }

        // 4. Update Button Interactivity
        foreach (var btn in swapIntoSlotButtons)
        {
            btn.interactable = isDecisionPhase && isAbilityAvailable;
        }
    }
    
    // Called when one of the 5 "Swap to Slot X" buttons is clicked
    private void OnSwapToSlotClicked(int slotIndex)
    {
        if (gameManager.playerRolledAbilities.Count > rolledAbilityIndex)
        {
            // Get the ability at the rolled index
            Ability abilityToSave = gameManager.playerRolledAbilities[rolledAbilityIndex];

            // 1. Save the ability to the permanent slot
            playerController.SaveRolledAbility(abilityToSave, slotIndex);
            
            // 2. Remove the saved ability from the rolled list
            // This is the crucial step that marks this roll as 'used' and causes the list to shift.
            gameManager.playerRolledAbilities.RemoveAt(rolledAbilityIndex);
        }
    }
}