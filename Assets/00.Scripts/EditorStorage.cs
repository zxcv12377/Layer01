using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorStorage : BaseMonoSingleton<EditorStorage>
{
    void Awake()
    {
        ResourceManager.CreateInstance();
        DataManager.CreateInstance();
    }
}
