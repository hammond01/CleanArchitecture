namespace ProductManager.Shared.Locks;

public interface IDistributedLockScope : IDisposable
{
    bool StillHoldingLock();
}
