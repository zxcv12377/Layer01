using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    Hero_Knight, New1, None
}

public class DataMgr : MonoBehaviour
{
    public Character currentCharacter;

    private static DataMgr Instance;
    public static DataMgr instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<DataMgr>();
                if(obj != null)
                {
                    Instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("Data Manager").AddComponent<DataMgr>();
                    Instance = newSingleton;
                }
            }
            return Instance;
        }
        private set
        {
            Instance = value;
        }
    }
    private void Awake()
    {
        var objs = FindObjectsOfType<DataMgr>();
        if(objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
