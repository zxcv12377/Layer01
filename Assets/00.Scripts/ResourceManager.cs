using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResourceManager : BaseSingleton<ResourceManager>, IBaseSingleton
{
    private Dictionary<string, GameObject> prefabs;
    public Dictionary<string, GameObject>.KeyCollection GetPrefabKeyCollection()
    {
        if (prefabs != null)
        {
            return prefabs.Keys;
        }
        return null;
    }

    private Dictionary<string, Sprite> sprites;

    public void OnCreateIntance()
    {
        Initialize();
    }

    public new void OnDestroyInstance() // 필요시 new 제거
    {
        Release();
    }
    public void Initialize()
    {
        prefabs = new Dictionary<string, GameObject>();
        sprites = new Dictionary<string, Sprite>();

        var gobjCollections = Resources.LoadAll<GameObejctCollection>(string.Empty);
        foreach (var c in gobjCollections)
        {
            foreach (var d in c.datas)
            {
                prefabs.Add(d.key, d.value);
            }
        }

        var spriteCollections = Resources.LoadAll<SpriteCollection>(string.Empty);
        foreach (var c in spriteCollections)
        {
            foreach (var d in c.datas)
            {
                sprites.Add(d.key, d.sprite);
            }
        }
#if UNITY_EDITOR
        Debug.Log(GetLog());

#endif
    }

    public string GetLog()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(string.Format("----- ResourceManger Log -----"));

        sb.AppendLine(string.Format("-- Sprite List"));
        if (sprites != null)
        {
            foreach (var s in sprites)
            {
                sb.AppendLine(string.Format("# Registed Key : {0}", s.Key));
            }
        }
        return sb.ToString();
    }

    public void Release()
    {
        sprites?.Clear();
        sprites = null;
    }

    public static bool TryGetSprite(string key, out Sprite sprite)
    {
        return Instance.sprites.TryGetValue(key, out sprite);
    }

    public static Sprite GetSprite(string key)  
    {
        Sprite sprite;
        if (Instance.sprites.TryGetValue(key, out sprite))
        {
            return sprite;
        }
        return null;
    }
}
