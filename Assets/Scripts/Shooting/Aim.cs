using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Aim : NetworkBehaviour
{
    private uint _owner;
    private Vector3 _target;
    private bool _isinitied;

    [SerializeField] GameObject _hitPersonVHX;
    [SerializeField] GameObject _hitWallVHX;
    [SerializeField] GameObject _destroyedOnAirVHX;

    float _speed;

    [Server]
    public void Initialize(uint owner, Vector3 target, float speed)
    {
        this._owner = owner;
        this._target = target;
        this._speed = speed;
        _isinitied = true;
    }

    [Server]
    private void SpawnParticle(GameObject _particleSystem)
    {
        GameObject newObj = Instantiate(_particleSystem, this.transform.position, this.transform.rotation);
        NetworkServer.Spawn(newObj);
    }

    [Command]
    private void SpawnParticleClient(GameObject _particleSystem)
    {
        SpawnParticle(_particleSystem);
    }

    public void Update()
    {
        if (_isinitied && isServer)
        {
            Vector3 wannaPos = (_target - transform.position).normalized;

            //this.transform.Translate(wannaPos * _speed);

            GetComponent<Rigidbody2D>().velocity = wannaPos * _speed * 100;

            this.transform.rotation = Quaternion.Euler(0, 0, CalcAngle() - 90);

            foreach (var item in Physics2D.OverlapCircleAll(transform.position, 0.5f))
            {
                PlayerController player = item.GetComponent<PlayerController>();

                if (player)
                {
                    if (player.netId != _owner)
                    {

                        if (isServer)
                        {
                            SpawnParticle(_hitPersonVHX);
                        }
                        else
                        {
                            SpawnParticleClient(_hitPersonVHX);
                        }

                        if (isServer)
                        {
                            player.TakeDamage(30f);
                        }
                        else
                        {
                            player.CmdTakeDamage(30f);
                        }

                        NetworkServer.Destroy(this.gameObject);
                    }
                }
            }

            if (Vector3.Distance(transform.position, _target) < 0.1f)
            {
                if (isServer)
                {
                    SpawnParticle(_destroyedOnAirVHX);
                }
                else
                {
                    SpawnParticleClient(_destroyedOnAirVHX);
                }

                NetworkServer.Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            if (collision.GetComponent<TilemapCollider2D>() != null)
            {

                if (isServer)
                {
                    SpawnParticle(_hitWallVHX);
                }
                else
                {
                    SpawnParticleClient(_hitWallVHX);
                }

                NetworkServer.Destroy(this.gameObject);
            }
        }
        finally
        {

        }
    }

    private float CalcAngle()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        return Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
    }
}