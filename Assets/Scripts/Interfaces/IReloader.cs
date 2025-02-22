public interface IReloader //interface for reload
{
    void StartReload(float reloadDuration);
    bool IsReloading { get; }
    void UpdateReload();
    float GetProgress();
    void StopReload();
}