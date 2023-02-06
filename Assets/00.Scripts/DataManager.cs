using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseSingleton<DataManager>, IBaseSingleton
{
    #region < DataStorages >

    private TileDataStorage m_TileDataStorage = new TileDataStorage();
    public static TileDataStorage tileDataStorage => Instance.m_TileDataStorage;

    #endregion

    private List<DataStorageBase> m_Storages = new List<DataStorageBase>();
    public void OnCreateIntance()
    {
        m_Storages.Add(m_TileDataStorage);
        m_Storages.ForEach(s => s.LoadData());


#if UNITY_EDITOR
        m_Storages.ForEach(s => Debug.Log(s.GetLog()));
#endif
    }

    public new void OnDestroyInstance()
    {
        
    }
}
