using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private WorldManagerController WMC;

    public void SelectCard1()
    {
        WMC.IsPause = false;
    }
    public void SelectCard2()
    {
        WMC.IsPause = false;
    }
    public void SelectCard3()
    {
        WMC.IsPause = false;
    }
    
    private void CardApply(int num)
    {
        switch (num)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }
}
