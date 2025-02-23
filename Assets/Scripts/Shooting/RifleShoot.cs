using Mirror;
using UnityEngine;

public class RifleShoot : IShoot
{
    private int currentAmmo = 30;
    private float reloadProgress = 0f;
    private float currentTime;

    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private GameObject _bullet;

    public GameManager gameManager;
    private bool isReloadComplete = false;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        UpdateAmmoUI();
        gameManager.reloadProgressSlider.value = reloadProgress;
        gameManager.reloadProgressSliderGJ.gameObject.SetActive(false);
        gameManager.ammoText.gameObject.SetActive(false);
    }
    private void Start()
    {
        gameManager.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        currentTime += Time.deltaTime;

        if (CanShoot(currentTime, 0.25f) && inputHandler.GetMouseButton() && !reloader.IsReloading)
        {
            Shoot();
        }

        ReloadWeapon();
    }

    public override void ReloadWeapon()
    {
        if (inputHandler.GetRButton() && currentAmmo < maxAmmo)
        {
            gameManager.reloadProgressSliderGJ.gameObject.SetActive(true);
            reloader.StartReload(3f);
        }

        if (reloader.IsReloading)
        {
            reloader.UpdateReload();
            reloadProgress = reloader.GetProgress();

            gameManager.reloadProgressSlider.value = reloadProgress;
            if (reloadProgress >= 0.99f)
            {
                isReloadComplete = true;
            }
        }

        if (isReloadComplete)
        {
            currentAmmo = maxAmmo;

            UpdateAmmoUI();
            reloadProgress = 0f;
            gameManager.reloadProgressSliderGJ.gameObject.SetActive(false);
            isReloadComplete = false;
        }
    }

    public override void Shoot()
    {
        if (currentAmmo > 0 && CanShoot(currentTime, 0.25f))
        {
            if (isServer)
            {
                Spawn(CalculateVector());
            }
            else
            {
                SpawnClient(CalculateVector());
            }

            currentAmmo--;
            currentTime = 0;
        }
        UpdateAmmoUI();
    }

    [Server]
    private void Spawn(Vector3 pos)
    {
        SpawnBullets(netId, pos, 0.15f, _bullet);
    }

    [Command]
    private void SpawnClient(Vector3 pos)
    {
        SpawnBullets(netId, pos, 0.15f, _bullet);
    }

    private Vector3 CalculateVector()
    {
        float minDist = 9f;
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        Vector3 direction = (pos - transform.position).normalized * minDist;
        return transform.position + direction;
    }

    private void UpdateAmmoUI()
    {
        gameManager.ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }
}
