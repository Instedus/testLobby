using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IShoot : NetworkBehaviour // main class for shooting
{
    protected IInputHandler inputHandler;
    protected IReloader reloader;

    protected void Awake()
    {
        // dependency injection
        inputHandler = new InputHandler();
        reloader = new Reloader();
    }

    [Server] // method for spawning on server
    public virtual void SpawnBullets(uint owner, Vector3 target, float speed, GameObject _bullet)
    {
        GameObject NewBullet = Instantiate(_bullet, transform.position, Quaternion.identity);

        NetworkServer.Spawn(NewBullet);

        Aim _aim = NewBullet.GetComponent<Aim>();

        _aim.Initialize(owner, target, speed);
    }

    [Command] // method for spawning on client
    public virtual void ClientSpawnBullets(uint owner, Vector3 target, float speed, GameObject _bullet)
    {
        SpawnBullets(owner, target, speed, _bullet);
    }

    // shoot method 
    public abstract void Shoot();
    //reload weapon method
    public abstract void ReloadWeapon();

    // return true if current time between shoots >= your rifle time
    public bool CanShoot(float curTime, float time)
    {
        return curTime >= time;
    }

    // return false if current ammo <= 0
    public bool CheckAmmo(float curAmmo)
    {
        return curAmmo > 0;
    }
}
