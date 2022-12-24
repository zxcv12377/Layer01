using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManagerController : MonoBehaviour
{
    [HideInInspector] public bool IsPause = false;

    [SerializeField] private GameObject btnManager;

    private void Update()
    {
        if (IsPause)
        {
            SelectCardVisible();
        }
        else
        {
            SelectCardHidden();
        }
    }

    private void SelectCardVisible()
    {
        btnManager.SetActive(IsPause);
        Time.timeScale = 0;
    }

    private void SelectCardHidden()
    {
        btnManager.SetActive(IsPause);
        Time.timeScale = 1;
    }
}
