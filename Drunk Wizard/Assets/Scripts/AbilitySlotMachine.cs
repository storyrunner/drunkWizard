using UnityEngine;
using System.Collections.Generic;

public class AbilitySlotMachine : MonoBehaviour
{
    public List<Ability> availableAbilities;

    // Modified event to broadcast a list of rolled abilities
    public delegate void AbilitiesRolledHandler(List<Ability> rolledAbilities);
    public event AbilitiesRolledHandler OnAbilitiesRolled; // Changed event name and signature

    public GameObject pickupPrefab;            // prefab with AbilityPickup attached
    public Transform spawnPoint;               // where the first ability will appear
    public float spawnOffset = 1.0f;           // distance between pickups when spawning multiple
    private List<AbilityObject> spawnedPickups = new List<AbilityObject>(); //stores spawned pickups, clears each new slot roll
    
    // Constant for how many abilities to roll
    private const int RollsPerTurn = 3;

    void Start()
    {
        if (availableAbilities == null || availableAbilities.Count == 0)
        {
            Debug.LogError($"{gameObject.name} Slot Machine has no abilities assigned!");
        }
    }

    // Now rolls the specified number of abilities.
    public void RollAbilities()
    {
        if (availableAbilities.Count == 0)
        {
            Debug.LogError("Slot Machine has no abilities to roll!");
            return;
        }

         spawnedPickups.Clear();

        List<Ability> rolledAbilities = new List<Ability>();

        for (int i = 0; i < RollsPerTurn; i++)
        {
            // Simulate rolling by picking a random ability
            int randomIndex = Random.Range(0, availableAbilities.Count);
            Ability rolledAbility = availableAbilities[randomIndex];
            rolledAbilities.Add(rolledAbility);

            Debug.Log($"Reel {i + 1} rolled: {rolledAbility.abilityName}");
        }

        //spawn pickups after they're rolled
        SpawnAbilityObjects(rolledAbilities);

        // Broadcast the result (using the modified event)
        if (OnAbilitiesRolled != null)
        {
            //sends the abilities in an ability array to OnAbilitiesRolled functions
            OnAbilitiesRolled(rolledAbilities);
        }
    }

    private void SpawnAbilityObjects(List<Ability> spawns)
    {
        for (int i = 0; i < spawns.Count; i++) //fix
        {
            Vector3 offset = new Vector3(i * spawnOffset, 0, 0);
            GameObject pickupObj = Instantiate(pickupPrefab, spawnPoint.position + offset, Quaternion.identity);
            pickupObj.transform.localScale = new Vector3(2f, 2f, 1f); // increase size 2x
            AbilityObject pickup = pickupObj.GetComponent<AbilityObject>();
            pickup.Initialize(spawns[i]);
            pickup.onClicked.AddListener(OnPickupSelected); // listen for clicks
            spawnedPickups.Add(pickup);

            Debug.Log($"Spawned pickup for {spawns[i].abilityName}, but it's inactive.");
        }

        //Destroy(spawnedPickups[0].gameObject);
        //spawnedPickups.RemoveAt(0); // optional: remove reference from list 
    }

    private void OnPickupSelected(AbilityObject selected)
    {
        Debug.Log($"Selected ability: {selected.abilityData.abilityName}");

        // Destroy the other 2
        foreach (var pickup in spawnedPickups)
        {
            if (pickup != selected)
            {
                Destroy(pickup.gameObject);
            }
        }
    }

}
