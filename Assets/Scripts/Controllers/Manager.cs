using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Manager : NetworkManager
{
    private bool isPlayerSpawned;
    private bool isNewPlayerConnected;

    public override void OnClientConnect(NetworkConnection conn)
    {        
        isNewPlayerConnected = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isPlayerSpawned && isNewPlayerConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPos = Input.mousePosition;
        spawnPos.z = 10;
        spawnPos = Camera.main.ScreenToWorldPoint(spawnPos);

        Position newPos = new Position() { vector = spawnPos };
        NetworkClient.Send(newPos);
        isPlayerSpawned = true;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        //NetworkServer.RegisterHandler<Position>(OnCreateCharacter);
    }
    public void OnCreateCharacter(NetworkConnectionToClient connection, Position pos)
    {
        GameObject newObj = Instantiate(playerPrefab, pos.vector, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(connection, newObj);
    }
    public struct Position : NetworkMessage
    {
        public Vector2 vector;
    }
}
