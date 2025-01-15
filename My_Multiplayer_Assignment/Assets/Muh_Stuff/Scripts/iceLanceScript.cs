using UnityEngine;
using Unity.Netcode;

public class iceLanceScript : NetworkBehaviour
{
    [SerializeField] GameObject Lance;
    float timer = 2; 
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 2;
            Lance.gameObject.name = "IceLance";
            ObjectPoolManager.ReturnToPool(Lance);
        }
    }
}
