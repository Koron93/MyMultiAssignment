using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // Reference to the networked projectile prefab
    [SerializeField] private Transform firePoint; // The point where the projectile spawns

    public override void OnNetworkSpawn()
    {
        // Ensure this script only runs for the owning client
        if (!IsOwner)
        {
            enabled = false; // Disable the shooting script on clients that don't own the object
        }
    }

    public void ShootUpdate()
    {
        if (IsOwner && Input.GetButtonDown("Fire1"))
        {
            // Only the owner (local client) can shoot
            ShootProjectile();
            print("got this far");
        }
    }

    // Function to handle the shooting logic
    void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Call a ServerRpc to spawn the projectile on the server
            print("Now im here");
            SpawnProjectileServerRpc(firePoint.position, firePoint.rotation);
        }
    }

    [ServerRpc]
    void SpawnProjectileServerRpc(Vector3 position, Quaternion rotation)
    {
        print("Server has received the request to spawn projectile");

        // Call the object pool to spawn the projectile on the server
        GameObject projectile = ObjectPoolManager.SpawnObject(projectilePrefab, position, rotation);

        // Ensure the projectile has a NetworkObject component and spawn it across the network
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        if (networkObject != null && !networkObject.IsSpawned)
        {
            networkObject.Spawn(); // This spawns the object across the network
        }
    }
}