using Mirror;
using UnityEngine;

public class SniperShoot : IShoot
{
    private int currentAmmo = 5;
    private float reloadProgress = 0f;
    private float currentTime;

    [SerializeField] private int maxAmmo = 5;
    [SerializeField] private GameObject _bullet;

    [SerializeField] private float _lineLength = 5f;
    private LineRenderer _lineRenderer;

    private bool isReloadComplete = false;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        UpdateAmmoUI();
        gameManager.reloadProgressSlider.value = reloadProgress;
        gameManager.reloadProgressSliderGJ.gameObject.SetActive(false);

        currentTime = 2.5f;
        SetLineSettings();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        currentTime += Time.deltaTime;

        if (CanShoot(currentTime, 3f) && inputHandler.GetMouseButton() && !reloader.IsReloading)
        {
            Shoot();
        }

        ReloadWeapon();

        UpdateLineRenderer();
    }

    public override void ReloadWeapon()
    {
        if (inputHandler.GetRButton() && currentAmmo < maxAmmo)
        {
            gameManager.reloadProgressSliderGJ.gameObject.SetActive(true);
            reloader.StartReload(6f);
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
        if (currentAmmo > 0 && CanShoot(currentTime, 3f))
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
        SpawnBullets(netId, pos, 0.35f, _bullet);
    }

    [Command]
    private void SpawnClient(Vector3 pos)
    {
        SpawnBullets(netId, pos, 0.35f, _bullet);
    }

    private Vector3 CalculateVector()
    {
        float minDist = 15f;

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        Vector3 direction = (pos - transform.position).normalized * minDist;

        return transform.position + direction;
    }

    private void UpdateLineRenderer()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        Vector3 direction = (mousePos - transform.position).normalized;

        Vector3 endPoint = transform.position + direction * _lineLength;

        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, endPoint);
    }

    private void SetLineSettings()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        Color lineColor = new Color(1, 1, 1, 0.2f);
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = lineColor;
        _lineRenderer.endColor = lineColor;

        _lineRenderer.positionCount = 2;
    }

    private void UpdateAmmoUI()
    {
        gameManager.ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }
}
