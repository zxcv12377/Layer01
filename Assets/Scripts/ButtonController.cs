using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private WorldManagerController WMC;

    public void SelectCard1()
    {
        Debug.Log("Select1111111111111");
        WMC.IsPause = false;
    }
    public void SelectCard2()
    {
        Debug.Log("Select2222222222222");
        WMC.IsPause = false;
    }
    public void SelectCard3()
    {
        Debug.Log("Select33333333333");
        WMC.IsPause = false;
    }
}
