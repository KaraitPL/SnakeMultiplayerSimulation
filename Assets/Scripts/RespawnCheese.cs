using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnCheese : MonoBehaviour
{
    [SerializeField] public GameObject cheesePrefab;
    public void DestroyAndSpawn()
    {
        Vector3 position = new Vector3(Random.Range(-20, 20), Random.Range(-10, 10), 0);
        GameObject cheese = Instantiate(cheesePrefab, position, transform.rotation);
        cheese.GetComponent<NetworkObject>().Spawn();
        Destroy(gameObject);
    }
}
