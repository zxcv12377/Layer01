using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance; // �̱����� ���
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
        if (Instance.poolingObejctQueue.Count > 0) // �� �� �ִ� ������Ʈ�� ������ ���
        {
            var obj = Instance.poolingObejctQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            obj.InvokeDestroy();
            return obj;
        }
        else // �� �� �ִ� ������Ʈ�� �������� ���� ���
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
