using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteCollection", menuName = "Collection/SpriteCollection")]
public class SpriteCollection : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public Data()
        {
            key = string.Empty;
            sprite = null;
        }

        public Data(string _key, Sprite _sprite)
        {
            key = _key;
            sprite = _sprite;
        }

        public string key;
        public Sprite sprite;

        public void SetData(Data _data)
        {
            key = _data.key;
            sprite = _data.sprite;
        }

        public void Initialize()
        {
            key = string.Empty;
            sprite = null;
        }
    }

    [SerializeField]
    public List<Data> datas = new List<Data>();

    public int Count { get { return datas.Count; } }

    /// <summary>
    /// 찾을 수 없는 데이터일 경우 DefaultData를 반환한다.
    /// </summary>
    /// <param name="_spriteName"></param>
    /// <returns></returns>
    public Data this[string _key]
    {
        get
        {
            foreach (var data in datas)
            {
                if (data.key.Equals(_key))
                {
                    return data;
                }
            }
            return null;
        }
    }

    public bool TryGetData(string _spriteName, out Data _data)
    {
        foreach (var data in datas)
        {
            if (data.key.Equals(_spriteName))
            {
                _data = data;
                return true;
            }
        }
        _data = null;
        return false;
    }

    public bool TryGetValue(string _spriteName, out Sprite _sprite)
    {
        foreach (var data in datas)
        {
            if (data.key.Equals(_spriteName))
            {
                _sprite = data.sprite;
                return true;
            }
        }

        _sprite = null;
        return false;
    }

    public bool ContainsKey(string _key)
    {
        foreach (var data in datas)
        {
            if (data.key.Equals(_key))
            {
                return true;
            }
        }
        return false;
    }

    public void AddData(Data _data)
    {
        datas.Add(_data);
    }

    public void AddToDictionary(Dictionary<string, Sprite> dic, bool _log = false)
    {
        foreach (var data in datas)
        {
            if (dic.ContainsKey(data.key))
            {
                Debug.LogErrorFormat("AddToDictionary : Already contain Key({0})", data.key);
            }
            else
            {
                dic.Add(data.key, data.sprite);
                if (_log) Debug.Log("AddToDictionary Key : " + data.key + " , Sprite : " + data.sprite);
            }
        }
    }

    public void CopyTo(SpriteCollection collection)
    {
        collection.datas.Clear();

        foreach (var d in datas)
        {
            collection.datas.Add(d);
        }
    }
}
