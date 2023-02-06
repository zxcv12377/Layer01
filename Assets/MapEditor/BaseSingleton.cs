using UnityEngine;

public interface IBaseSingleton
{
    void OnCreateIntance();
    void OnDestroyInstance();
}

public abstract class BaseSingleton<T> where T : class, IBaseSingleton, new()
{
    private static T instance;
    public static T Instance => instance;

    public static T CreateInstance()
    {
        if(instance == null)
        {
            instance = new T();
            instance.OnCreateIntance();
        }
        return instance;
    }

    public static void OnDestroyInstance()
    {
        instance?.OnDestroyInstance();
        instance = null;
    }
}

public abstract class BaseMonoSingleton<T> : MonoBehaviour where T : class
{
    public static T Instance { get; private set; }

    public static T1 GetIntance<T1>() where T1 : class, T
    {
        return Instance as T1;
    }

    [SerializeField] private bool isDonDestroyLoadScene = true;
    public bool IsDestroyLoadScene => isDonDestroyLoadScene;

    protected virtual void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
            if (IsDestroyLoadScene) DontDestroyOnLoad(gameObject);
        }
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
