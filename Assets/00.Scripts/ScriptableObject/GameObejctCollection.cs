using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectCollection", menuName = "Collection/GameObjectCollection")]
public class GameObejctCollection : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public string key;
        public GameObject value;
    }

    [SerializeField] private List<Data> m_Datas;
    public List<Data> datas { get { return m_Datas; } private set { m_Datas = value; } }

    public Data this[string key]
    {
        get
        {
            foreach(var data in datas)
            {
                if (data.key.Equals(key))
                {
                    return data;
                }
            }
            return null;
        }
    }

    public bool TryGetData(string key, out Data data)
    {
        foreach(var d in datas)
        {
            if (d.key.Equals(key))
            {
                data = d;
                return true;
            }
        }

        data = null;
        return false;
    }

    public bool TryGetValue(string key, out GameObject gobj)
    {
        foreach(var d in datas)
        {
            if (d.key.Equals(key))
            {
                gobj = d.value;
                return true;
            }
        }

        gobj = null;
        return false;
    }
    public bool ContainsKey(string key)
    {
        foreach (var data in datas)
        {
            if (data.key.Equals(key))
            {
                return true;
            }
        }
        return false;
    }

    public void AddData(Data data)
    {
        datas.Add(data);
    }
}
