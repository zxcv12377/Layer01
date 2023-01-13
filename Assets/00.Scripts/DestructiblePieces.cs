using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DestructiblePieces : MonoBehaviour
{
    private IObjectPool<DestructiblePieces> managedPool;

    public void SetManagedPool(IObjectPool<DestructiblePieces> pool)
    {
        managedPool = pool;
    }
    public void destroytime()
    {
        Invoke("DestroyPieces", 5f);
    }
    public void DestroyPieces()
    {
        managedPool.Release(this);
    }
}
