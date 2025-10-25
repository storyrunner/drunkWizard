using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PhaseControlUI : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController playerController;

    [Header("Decision Phase Buttons")]
    public Button discardAllButton; 

    [Header("Select Phase Buttons")]
    public Button confirmChainButton; 
    public TextMeshProUGUI chainPreviewText; // To show the abilities currently selected in the chain
    public Image[] chainPreviewIcons = new Image[5];

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerController = FindFirstObjectByType<PlayerController>();

        if (gameManager == null || playerController == null) { enabled = false; return; }

        // Listener for the Discard All button
        discardAllButton.onClick.AddListener(OnDiscardAllClicked);
        
        // Listener for the Confirm Chain button
        confirmChainButton.onClick.AddListener(playerController.ConfirmAbilityChain);
    }

    void Update()
    {
        // --- Decision Phase Buttons (Save/Discard Rolls) ---
        bool isDecisionPhase = gameManager.CurrentState == GameManager.GameState.PlayerSlotDecision;
        discardAllButton.gameObject.SetActive(isDecisionPhase);

        // Discard All button instantly ends the slot phase.
        // It's the only way to end the decision phase after saving 0 to 3 abilities.
        if (isDecisionPhase)
        {
            // The button is interactable as long as we are in the decision phase.
            discardAllButton.interactable = true; 
        }

        // --- Select Phase Buttons (Confirm Combat Chain) ---
        bool isSelectPhase = gameManager.CurrentState == GameManager.GameState.PlayerAbilitySelect;
        confirmChainButton.gameObject.SetActive(isSelectPhase);
        
        // Update Chain Preview (Assuming PlayerController exposes the chain list)
        if (isSelectPhase)
        {
            var chain = playerController.GetSelectedAbilityChain();
            string chainNames = string.Join(" > ", chain.Select(a => a.abilityName));
            chainPreviewText.text = $"Chain: {chainNames}";

            for (int i = 0; i < chainPreviewIcons.Length; i++)
        {
            if (i < chain.Count)
            {
                // Set the sprite from the ability object
                chainPreviewIcons[i].sprite = chain[i].abilityIcon;
                chainPreviewIcons[i].enabled = true;
            }
            else
            {
                // Hide empty slots in the chain preview
                chainPreviewIcons[i].enabled = false;
            }
        }
            
            // Allow confirmation only if the player has selected at least one ability
            confirmChainButton.interactable = chain.Count > 0;
        }
        else
        {
            chainPreviewText.text = "";
            foreach(var img in chainPreviewIcons) { img.enabled = false; }
        }
    }

    private void OnDiscardAllClicked()
    {
        // Clear any remaining rolled abilities
        gameManager.playerRolledAbilities.Clear();
        // Move to the next phase (Ability Select)
        playerController.FinalizeSlotDecision();
    }
}