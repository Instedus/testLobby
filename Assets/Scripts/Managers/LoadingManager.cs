using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
public class LoadingManager : MonoBehaviour
{
    NetworkManager manager;    

    [Tooltip("mark isClient when build for WebglVersion or mark isServer when build for server (dont mark both of them)")]
    [SerializeField] bool isServer;
    [Tooltip("mark isClient when build for WebglVersion or mark isServer when build for server (dont mark both of them)")]
    [SerializeField] bool isClient;

    event Action<string> onInitialize;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
        manager.autoCreatePlayer = false;        
    }

    private void Start()
    {
        try
        {
            if (isServer)
            {
                manager.StartServer();
            }
            else if(isClient)
            {
                manager.StartClient();
                onInitialize += LoadSceneString;
            }
        }
        catch(Exception e)
        {
            SceneManager.LoadScene(manager.offlineScene);
            Debug.LogWarning(e.Message);
        }

        //change scene after initialization
        onInitialize?.Invoke("Lobby Scene");
        //onInitialize?.Invoke("Scene Ilgizar");
    }

    void LoadSceneString(string nameOf)
    {
        SceneManager.LoadScene(nameOf);
    }
}
