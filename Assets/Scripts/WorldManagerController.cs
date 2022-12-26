using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManagerController : MonoBehaviour
{
    [HideInInspector] public bool IsPause = false;

    [SerializeField] private GameObject CardSet;

    [SerializeField] private GameObject[] card;
    private Text[] cardtxt = new Text[3];

    private List<int> randList = new List<int>();


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
        Time.timeScale = 0;
        CardSet.SetActive(IsPause);
        Unduplicatied();
        for(int i = 0; i < 3; i++)
        {
            Cardtxtchg(randList[i], i);
        }
    }

    private void SelectCardHidden()
    {
        randList.Clear();
        CardSet.SetActive(IsPause);
        Time.timeScale = 1;
    }

    private void Cardtxtchg(int num, int n)
    {
        cardtxt[n] = card[n].GetComponent<Text>();
        switch (num)
        {
            case 0:
                cardtxt[n].text = "0번 선택지 입니다.";
                break;
            case 1:
                cardtxt[n].text = "1번 선택지 입니다.";
                break;
            case 2:
                cardtxt[n].text = "2번 선택지 입니다.";
                break;
            case 3:
                cardtxt[n].text = "3번 선택지 입니다.";
                break;
            default:
                break;
        }
    }

    private void Unduplicatied()
    {
        int N = 4;
        List<int> list = new List<int>();
        for(int i = 0; i < N; ++i)
        {
            list.Add(i);
        }

        for(int i = 0; i < 3; i++)
        {
            int a = Random.Range(0,N-i);
            list.RemoveAt(a);
            randList.Add(a);
        }
    }
}
