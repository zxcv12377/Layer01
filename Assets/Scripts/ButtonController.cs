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
            case 0: // �ִ�ü�� 1ȸ���ϸ鼭 ����ü�µ� 1ȸ��
                Debug.Log("�ִ�ü�� 1ȸ��!");
                CC.maxHP++;
                CC.currentHP++;
                break;
            case 1: // ������ 1����
                Debug.Log("������ 1����!");
                CC.Damage++;
                break;
            case 2: // ����ü�� 1����
                Debug.Log("����ü�� 1����!");
                if(CC.currentHP < CC.maxHP)
                {
                    CC.currentHP++;
                }
                else
                {
                    Debug.Log("�̹� �ִ�ü�� �Դϴ�.");
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
