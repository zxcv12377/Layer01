using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    Hero_Knight, New1, None
}

public class DataMgr : MonoBehaviour
{
    public static DataMgr instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) return;
        DontDestroyOnLoad(gameObject);
    }

    public Character currentCharacter;
}
