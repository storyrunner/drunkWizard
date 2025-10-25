using UnityEngine;
using UnityEngine.Events;

public class AbilityObject : MonoBehaviour
{
    [HideInInspector] public Ability abilityData; //hidden so its dynamic
    [SerializeField] private SpriteRenderer spriteRenderer;
    public UnityEvent<AbilityObject> onClicked; // variable to tell if object is pressed

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Initializes this pickup with a given ability's data.
    /// Sets visuals but does not make it interactable yet.
    /// </summary>
    public void Initialize(Ability newAbility)
    {
        abilityData = newAbility;
        gameObject.name = $"Pickup_{newAbility.abilityName}";

        // Make sure we use the instance's own SpriteRenderer
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // Error catch
        if (spriteRenderer == null)
        {
            Debug.LogError($"AbilityObject '{name}' has no SpriteRenderer component!");
            return;
        }

        // set the icon
        if (newAbility.abilityIcon != null)
            spriteRenderer.sprite = newAbility.abilityIcon;

        spriteRenderer.sortingOrder = 15;
        spriteRenderer.enabled = true;

        //adding collider
        AddColliderForClickDetection();
    }

    /// <summary>
    /// Makes this pickup active for interaction.
    /// </summary>

    //function to tell when it is pressed
    private void OnMouseDown() // called when clicked (if object has collider)
    {
        onClicked?.Invoke(this);
    }

    //function to add collider
    private void AddColliderForClickDetection()
    {
        // If it already has a collider, don’t add another
        if (GetComponent<Collider2D>() != null)
            return;

        // Automatically pick a collider type (BoxCollider2D is usually best for UI-like objects)
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();

        // Optionally, resize collider to fit the sprite
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            collider.size = spriteRenderer.sprite.bounds.size * 2.0f;
            collider.offset = spriteRenderer.sprite.bounds.center;

            Debug.Log($"Collider added. Size: {collider.size}, Offset: {collider.offset}");

            Vector3 colliderWorldPos = transform.TransformPoint(collider.offset);
            Debug.Log($"GameObject position: {transform.position}, Collider world position: {colliderWorldPos}");
        }

        collider.isTrigger = false; // make sure clicks register
    }
}
