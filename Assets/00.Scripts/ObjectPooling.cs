using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance; // 싱글톤을 사용
    [SerializeField] private GameObject poolingObjectPrefab;
    private Queue<DeathHand> poolingObejctQueue = new Queue<DeathHand>();
    private void Awake()
    {
        Instance = this;
        Initialize(1);
    }

    private DeathHand CreateMissile()
    {
        var newObj = Instantiate(poolingObjectPrefab, transform).GetComponent<DeathHand>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private void Initialize(int count)
    {
        for(int i = 0; i < count; i++)
        {
            poolingObejctQueue.Enqueue(CreateMissile());
        }
    }

    public static DeathHand GetObject()
    {
        if (Instance.poolingObejctQueue.Count > 0) // 줄 수 있는 오브젝트가 존재할 경우
        {
            var obj = Instance.poolingObejctQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            obj.InvokeDestroy();
            return obj;
        }
        else // 줄 수 있는 오브젝트가 존재하지 않을 경우
        {
            var newObj = Instance.CreateMissile();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            newObj.InvokeDestroy();
            return newObj;
        }
    }

    public static void ReturnObject(DeathHand missile)
    {
        missile.gameObject.SetActive(false);
        missile.transform.SetParent(Instance.transform);
        Instance.poolingObejctQueue.Enqueue(missile);
    }
}
