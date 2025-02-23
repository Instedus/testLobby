using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTest : MonoBehaviour
{
    GameManager manager;


    private void Start()
    {
        manager = GetComponent<GameManager>();
        DontDestroyOnLoad(manager);
        DontDestroyOnLoad(manager.ammoText.GetComponentInParent<Canvas>().gameObject);
    }
}
