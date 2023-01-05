using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSelectButtonController : MonoBehaviour
{
    
    [SerializeField] private GameObject uitxt;
    private Text txt;
    [SerializeField] private GameObject[] SelectChracter;
    [SerializeField] private GameObject Warning;
    private int[] SelectCnt = {0, 0};

    private void Start()
    {
        txt = uitxt.GetComponent<Text>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("enter") || Input.GetMouseButtonDown(0))
        {
            Warning.SetActive(false);
        }

        if(EventSystem.current.currentSelectedGameObject.name == "Hero_Knight" && SelectCnt[0] == 0)
        {
            txt.text = "전사";
            SelectCnt[0] = 1;
            SelectCnt[1] = 0;
            print(EventSystem.current.currentSelectedGameObject.name);
        }

        else if (EventSystem.current.currentSelectedGameObject.name == "New1" && SelectCnt[1] == 0)
        {
            txt.text = "미확인";
            SelectCnt[0] = 0;
            SelectCnt[1] = 1;
            print(EventSystem.current.currentSelectedGameObject.name);
        }
    }

    public void Gamestart()
    {
        for(int i = 0; i<SelectCnt.Length; i++)
        {
            if(SelectCnt[i] == 1)
            {
                if (i == 1)
                {
                    Warning.SetActive(true);
                }
            }

        }
    }

    public void back()
    {

    }
}
