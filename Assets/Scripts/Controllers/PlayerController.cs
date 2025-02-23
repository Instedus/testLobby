using UnityEngine;
using Mirror;
using UnityEngine.UI;
using MirrorBasics;
using UnityEngine.SceneManagement;


public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Slider healthSlider;

    [SyncVar(hook = nameof(OnHealthChanged))]
    private float health = 100f;

    private float maxHealth = 100f;

    private Vector2 input;
    private float speed = 2f;

    [SerializeField] private GameObject _Bullet;
    [SerializeField] private GameObject _deathVHX;

    private Vector3 _camera_player_offset;
    private float _smoothSpeed = 0.125f;
    [SerializeField] float _offset;

    private GameObject _camera;

    private GameManager gameManager;

    Player player;

    private void Awake()
    {
        _camera = Camera.main.gameObject;
        player = GetComponent<Player>();
    }

    private void Start()
    {
        if (healthSlider)
        {
            healthSlider.maxValue = 100f;
            healthSlider.value = health;
        }

        if (isServer)
        {
            SetRifleShootOff(false);
        }
        else
        {
            SetRifleShootOffClient(false);
        }
        player.OnSceneChangeSpawn += OnSceneChange;
    }

    [Server]
    void SetRifleShootOff(bool mode)
    {
        GetComponent<RifleShoot>().enabled = mode;
    }

    [Command]
    void SetRifleShootOffClient(bool mode)
    {
        SetRifleShootOff(mode);
    }

    void OnSceneChange()
    {
        if (isServer)
        {
            SetRifleShootOff(true);
        }
        else
        {
            SetRifleShootOffClient(true);
        }        
    }

    GameObject FindObjectByTagInAllScenes(string tag)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject[] rootObjects = scene.GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                Transform[] children = root.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child.CompareTag(tag))
                    {
                        return child.gameObject;
                    }
                }
            }
        }
        return null;
    }



    private void Update()
    {
        if (!isLocalPlayer)
            return;

        GetInput();
        Move();
        RotateSpritePLayer();
        MoveCamera();
        Damage();
    }

    private void MoveCamera()
    {
        Vector3 wannaPos = new Vector3(transform.position.x, transform.position.y, -10);
        _camera.transform.position = wannaPos;
    }

    #region HealthSystem
    [Server]
    public void TakeDamage(float damage)
    {
        health -= damage;



        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    [Command]
    public void CmdTakeDamage(float damage)
    {
        TakeDamage(damage);
    }

    public void OnHealthChanged(float oldHealth, float newHealth)
    {
        if (healthSlider)
        {
            healthSlider.value = newHealth;
        }
    }
    #endregion

    private void RotateSpritePLayer()
    {
        Ray ray = _camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, transform.position);

        if (plane.Raycast(ray, out float hit))
        {
            Vector3 mousePos = ray.GetPoint(hit);
            Vector3 direction = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + _offset));
        }
    }    

    private void Damage()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (isServer)
            {
                TakeDamage(30f);
            }
            else
            {
                CmdTakeDamage(30f);
            }
        }
    }

    private void Die()
    {
        if (_deathVHX)
        {
            GameObject newObj = Instantiate(_deathVHX, this.transform.position, Quaternion.identity);
            NetworkServer.Spawn(newObj);
        }
        NetworkServer.Destroy(this.gameObject);
    }

    [Server]
    public void SpawnBullet(uint owner, Vector3 target)
    {
        GameObject NewBullet = Instantiate(_Bullet, transform.position, Quaternion.identity);
        NetworkServer.Spawn(NewBullet);
        NewBullet.GetComponent<Aim>().Initialize(owner, target, 0.09f);
    }

    [Command]
    public void ClientSpawnBullet(uint owner, Vector3 target)
    {
        SpawnBullet(owner, target);
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 10f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            if (isServer)
            {
                SpawnBullet(netId, pos);
            }
            else
            {
                ClientSpawnBullet(netId, pos);
            }
        }
    }

    private void Move()
    {
        Vector3 move = new Vector3(input.x, input.y, 0);
        transform.position += move * Time.deltaTime * speed;
    }

    private void GetInput()
    {
        input = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }
}