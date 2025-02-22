using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LobbyInfo : MonoBehaviour
{

    public string lobbyName;
    public int maxPlayers;
    public int currentPlayers;

    public string roomId;

    public List<NetworkConnectionToClient> players;

    public void AddPlayers(NetworkConnectionToClient conn)
    {
        players.Add(conn);
        currentPlayers++;
    }

    public void RemovePlayers(NetworkConnectionToClient conn)
    {
        players.Remove(conn);
        currentPlayers--;
    }

    public LobbyInfo(string name, int max)
    {
        this.lobbyName = name;
        this.maxPlayers = max;
        currentPlayers = 0;
        players = new List<NetworkConnectionToClient>();
    }


}
