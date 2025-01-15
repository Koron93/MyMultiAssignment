using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab;  // Reference to the projectile prefab
    [SerializeField] private Transform firePoint;     // The point where the projectile spawns (usually near the camera or weapon)

    public override void OnNetworkSpawn()
    {
        // Ensure the script only runs for the owning client
        if (!IsOwner)
        {
            enabled = false;  // Disable the shooting script on clients that don't own the object
        }
    }

    public void ShootUpdate()
    {
        if (IsOwner && Input.GetButtonDown("Fire1"))
        {
            // Only the owner (local client) can shoot
            ShootProjectile();
        }
    }

    // Function to handle the shooting logic
    void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Call the networked method to spawn the projectile
            SpawnProjectileServerRpc(firePoint.position, firePoint.rotation);
        }
    }

    // ServerRpc to spawn the projectile on the server and replicate it to all clients
    [ServerRpc]
    void SpawnProjectileServerRpc(Vector3 position, Quaternion rotation)
    {
        // Instantiate the projectile on the server
        GameObject projectile = ObjectPoolManager.SpawnObject(projectilePrefab, new Vector3(position.x, 1.1f, position.z), rotation);

        // Spawn it across the network
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        if (networkObject != null && !networkObject.IsSpawned)
        {
            networkObject.Spawn();  // This will spawn the object on all clients
        }
    }
}