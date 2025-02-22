using UnityEngine;

public class Reloader : IReloader
{
    private float currentTimeReload;
    private bool isReloading;
    private float reloadTime;

    public void StartReload(float reloadDuration)
    {
        isReloading = true;
        currentTimeReload = 0;
        reloadTime = reloadDuration;
    }

    public bool IsReloading => isReloading;

    public void UpdateReload()
    {
        if (isReloading)
        {
            currentTimeReload += Time.deltaTime;
            if (currentTimeReload > reloadTime)
            {
                isReloading = false;
            }
        }
    }

    public float GetProgress()
    {
        if (isReloading)
        {
            return Mathf.Clamp01(currentTimeReload / reloadTime);
        }
        return 0f;
    }

    public void StopReload()
    {
        isReloading = false;
    }
}