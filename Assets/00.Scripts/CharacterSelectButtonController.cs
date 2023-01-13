using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadScene("Level1");
        }
        else if (DataMgr.instance.currentCharacter == Character.New1)
        {
            activate = true;
            Warning.SetActive(true);
        }
    }

    public void Back()
    {

    }
}
