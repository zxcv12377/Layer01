using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectButtonController : MonoBehaviour
{
    
    [SerializeField] private GameObject Warning;
    private bool activate = false;

    private void Update()
    {
        if (activate)
        {
            if (Input.GetKeyDown("enter") || Input.GetMouseButtonDown(0))
            {
                activate = false;
                Warning.SetActive(false);
            }
        }
    }

    public void GameStart()
    {
        if(DataMgr.instance.currentCharacter == Character.Hero_Knight)
        {
            print("Àü»ç");
        }
        else if (DataMgr.instance.currentCharacter == Character.New1)
        {
            print("¾ÈµÊ");
            activate = true;
            Warning.SetActive(true);
        }
    }

    public void Back()
    {

    }
}
