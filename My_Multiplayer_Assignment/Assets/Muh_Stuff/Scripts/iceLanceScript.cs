using UnityEngine;
using Unity.Netcode;

public class IceLanceScript : NetworkBehaviour
{
    [SerializeField] GameObject Lance;  // Make sure this is assigned in the inspector or instantiated beforehand
    float timer = 2;

    private void Update()
    {
        if (!IsHost || Lance == null) return; // Ensure we only process if we're the host and the Lance is assigned

        timer -= Time.deltaTime;

        if (timer < 0)
        {

            // Rename object before returning it to the pool
            Lance.gameObject.name = "IceLance";

            // Call the server-side despawn method, pass the NetworkObject ID instead of the GameObject
            DespawnProjectileServerRpc(Lance.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    // ServerRpc now receives the NetworkObjectId
    [ServerRpc(RequireOwnership = false)]
    void DespawnProjectileServerRpc(ulong networkObjectId)
    {
        if (IsHost)
        {
            // Find the NetworkObject using the received NetworkObjectId
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];

            // Return the object to the pool, assuming you have an object pooling system in place
            // You will still need to handle pooling separately for networked objects
            ObjectPoolManager.ReturnToPool(networkObject.gameObject);  // Ensure pooling is implemented correctly
        }
    }
}