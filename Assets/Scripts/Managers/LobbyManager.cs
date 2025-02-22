using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public List<LobbyInfo> activeLobbies = new List<LobbyInfo>();   

    public void CreateLobby(string name, int max)
    {
        LobbyInfo newLobby = new LobbyInfo(name, max);

        activeLobbies.Add(newLobby);

        Debug.Log($"Lobby created: {name}");
    }

    public void JoinLobby(string name, NetworkConnectionToClient conn)
    {
        LobbyInfo lobby = activeLobbies.Find(l => l.lobbyName == name);

        if (lobby && lobby.currentPlayers <= lobby.maxPlayers)
        {
            lobby.AddPlayers(conn);
        }
    }

    public void LeaveLobby(string name, NetworkConnectionToClient conn)
    {
        LobbyInfo lobby = activeLobbies.Find(l => l.lobbyName == name);

        if (lobby && lobby.currentPlayers <= lobby.maxPlayers)
        {
            lobby.RemovePlayers(conn);
        }
    }
}
