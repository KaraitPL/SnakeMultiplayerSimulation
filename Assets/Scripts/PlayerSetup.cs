using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefabA; //add prefab in inspector
    [SerializeField] private GameObject playerPrefabB; //add prefab in inspector
    NetworkObject netObj;
    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientId, int prefabId)
    {
        GameObject newPlayer;
        if (prefabId == 1)
            newPlayer = (GameObject)Instantiate(playerPrefabA);
        else
            newPlayer = (GameObject)Instantiate(playerPrefabB);
        netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
        else if (Input.GetKeyDown(KeyCode.S))
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 2);
    }
}

