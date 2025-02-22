using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTest : NetworkBehaviour
{
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        
            Transform spawnPos = GameObject.FindGameObjectWithTag("spawn").transform;

            if (isServer)
            {
                SpawnPlayer(spawnPos, playerPrefab);
            }
            else
            {
                ClientSpawnPlayer(spawnPos, playerPrefab);
            }
            //isSpawned = true;
        
    }

    [Server]
    private void SpawnPlayer(Transform pos, GameObject playerPrefab)
    {
        GameObject newPlayer = Instantiate(playerPrefab, pos.position, Quaternion.identity);
        NetworkServer.Spawn(newPlayer);
        //this.gameObject.transform.position = pos.position;
        //newPlayer.transform.SetParent(this.gameObject.transform);
    }
    [Command]
    private void ClientSpawnPlayer(Transform pos, GameObject playerPrefab)
    {
        SpawnPlayer(pos, playerPrefab);
    }
}
