using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private WorldManagerController WMC;

    [SerializeField] private GameObject PauseInterface;

    private CharacterController CC;
    private GameObject player;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            yield return new WaitForSeconds(0.2f);
            player = GameObject.FindWithTag("Player");
            CC = player.GetComponent<CharacterController>();
        }
    }

    public void Continue()
    {
        Time.timeScale = 1;
        CC.isPause = false;
        PauseInterface.SetActive(false);
    }

    public void CharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void GameEnd()
    {
        SceneManager.LoadScene("Main");
    }

    public void SelectCard1()
    {
        if (WMC.IsPause)
        {
            CardApply(WMC.randList[0]);
            WMC.SelectCardHidden();
            WMC.IsPause = false;
        }
    }
    public void SelectCard2()
    {
        if (WMC.IsPause)
        {
            CardApply(WMC.randList[1]);
            WMC.SelectCardHidden();
            WMC.IsPause = false;
        }
    }
    public void SelectCard3()
    {
        if (WMC.IsPause)
        {
            CardApply(WMC.randList[2]);
            WMC.SelectCardHidden();
            WMC.IsPause = false;
        }
    }
    
    private void CardApply(int num)
    {
        switch (num)
        {
            case 0: // 최대체력 1회복하면서 현재체력도 1회복
                Debug.Log("최대체력 1회복!");
                CC.maxHP++;
                CC.currentHP++;
                break;
            case 1: // 데미지 1증가
                Debug.Log("데미지 1증가!");
                CC.Damage++;
                break;
            case 2: // 현재체력 1증가
                Debug.Log("현재체력 1증가!");
                if(CC.currentHP < CC.maxHP)
                {
                    CC.currentHP++;
                }
                else
                {
                    Debug.Log("이미 최대체력 입니다.");
                }
                break;
            case 3:
                Debug.Log(num);
                break;
            default:
                break;
        }
    }
}
