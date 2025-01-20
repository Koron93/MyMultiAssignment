using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private Healthbar healthBar;  // Reference to the health bar UI

    // Network variable to keep track of health across the network
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(10f);

    // Property to get and set health
    public float CurrentHealth
    {
        get => currentHealth.Value;
        set
        {
            if (IsServer) // Make sure only the server can modify health
            {
                currentHealth.Value = Mathf.Clamp(value, 0f, maxHealth);
            }
        }
    }

    // ServerRpc function to apply damage from the client
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (IsServer)
        {
            Debug.Log("Damage applied to player: " + damage);
            CurrentHealth -= damage;  // Apply the damage

            // Update health bar on all clients (including the server)
            TakeDamageClientRpc();

            if (CurrentHealth <= 0)
            {
                OnDeathClientRpc();
            }
        }
    }

    // ClientRpc to notify the client that the player has taken damage
    [ClientRpc]
    public void TakeDamageClientRpc()
    {
        // Ensure that only the owning client updates their health bar
        if (IsOwner && healthBar != null)
        {
            // Update health bar based on current health and max health
            healthBar.UpdateHealthBar(currentHealth.Value / maxHealth);
        }
    }

    // ClientRpc to notify clients of death
    [ClientRpc]
    private void OnDeathClientRpc()
    {
        if (IsClient)
        {
            print("Haha you're dead");

            // Handle player death (e.g., stop movement, show death UI, etc.)
            // You can also trigger player death animations here
        }
    }
    private void OnClientConnected(ulong clientid)
    {
        if (IsClient)
        {
            // Find the Healthbar if it's not already assigned
            if (healthBar == null)
            {
                GameObject healthBarObject = GameObject.Find("Health"); // Adjust as necessary
                if (healthBarObject != null)
                {
                    healthBar = healthBarObject.GetComponent<Healthbar>();
                }
            }

            // Initial health bar update (if applicable, based on the initial health value)
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(currentHealth.Value / maxHealth);
            }
        }
    }
}