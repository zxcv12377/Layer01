using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance; // �̱����� ���
    [SerializeField] private GameObject poolingObjectPrefab;
    private Queue<GuidedMissile> poolingObejctQueue = new Queue<GuidedMissile>();
    private void Awake()
    {
        Instance = this;
    }

    private GuidedMissile CreateMissile()
    {
        var newObj = Instantiate(poolingObjectPrefab, transform).GetComponent<GuidedMissile>();
        newObj.gameObject.SetActive(true);
        return newObj;
    }

    private void Initialize(int count)
    {
        for(int i = 0; i < count; i++)
        {
            poolingObejctQueue.Enqueue(CreateMissile());
        }
    }

    public static GuidedMissile GetObject(Transform target)
    {
        if (Instance.poolingObejctQueue.Count > 0) // �� �� �ִ� ������Ʈ�� ������ ���
        {
            var obj = Instance.poolingObejctQueue.Dequeue();
            if(obj.target == null)
            {
                obj.target = target;
            }
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            
            return obj;
        }
        else // �� �� �ִ� ������Ʈ�� �������� ���� ���
        {
            var newObj = Instance.CreateMissile();
            if (newObj.target == null)
            {
                newObj.target = target;
            }
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }

    public static void ReturnObject(GuidedMissile missile)
    {
        missile.gameObject.SetActive(false);
        missile.transform.SetParent(Instance.transform);
        Instance.poolingObejctQueue.Enqueue(missile);
    }
}
