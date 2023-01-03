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
    private Button btn;
    private int[] SelectCnt = {0, 0};

    private void Start()
    {
        txt = uitxt.GetComponent<Text>();
        //btn = GetComponent<Button>(); 맵을 사용할까 말까 고민중
    }

    private void Update()
    {
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

    public void Select()
    {
        
    }

    public void Gamestart()
    {

    }

    public void back()
    {

    }
}
