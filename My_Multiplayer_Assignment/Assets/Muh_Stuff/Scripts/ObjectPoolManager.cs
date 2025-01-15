using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 SpawnPosition, Quaternion SpawnRotation)
    {
        PooledObjectInfo pool = ObjectPools.Find(n => n.LookupString == objectToSpawn.name);

        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        GameObject SpawnAbleObject = pool.InactiveObjects.FirstOrDefault();

        if (SpawnAbleObject == null)
        {
            SpawnAbleObject = Instantiate(objectToSpawn, SpawnPosition, SpawnRotation);
        }

        else
        {
            SpawnAbleObject.transform.position = SpawnPosition;
            SpawnAbleObject.transform.rotation = SpawnRotation;
            pool.InactiveObjects.Remove(SpawnAbleObject);
            SpawnAbleObject.SetActive(true);
        }
        return SpawnAbleObject;
    }
    public static void ReturnToPool(GameObject obj)
    {
        string goName = obj.name;//.Substring(0, obj.name.Length -7);
        PooledObjectInfo pool = ObjectPools.Find(n => n.LookupString == goName);
        if (pool == null)
        {
            Debug.Log("Trying to pool unpooled items");
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }
}
public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}