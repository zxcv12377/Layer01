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

    [HideInInspector] public List<int> randList = new List<int>();

    [SerializeField] private int cardAmount;

    public void SelectCardVisible()
    {
        Time.timeScale = 0;
        CardSet.SetActive(true);
        Unduplicatied();
        for(int i = 0; i < 3; i++)
        {
            Cardtxtchg(randList[i], i);
        }
    }

    public void SelectCardHidden()
    {
        randList.Clear();
        CardSet.SetActive(false);
        Time.timeScale = 1;
    }

    private void Cardtxtchg(int num, int n)
    {
        cardtxt[n] = card[n].GetComponent<Text>();
        switch (num)
        {
            case 0:
                cardtxt[n].text = "최대체력을 한칸 획득합니다.";
                break;
            case 1:
                cardtxt[n].text = "데미지가 증가합니다.";
                break;
            case 2:
                cardtxt[n].text = "체력을 한칸 회복합니다.";
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
        List<int> list = new List<int>();
        for(int i = 0; i < cardAmount; ++i)
        {
            list.Add(i);
        }

        for(int i = 0; i < 3; i++)
        {
            int a = Random.Range(0,cardAmount-i);
            randList.Add(list[a]);
            list.RemoveAt(a);
        }
    }
}
