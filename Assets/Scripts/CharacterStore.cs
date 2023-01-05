using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStore : MonoBehaviour
{
    [SerializeField] private GameObject uitxt;
    private Text txt;
    [SerializeField] private Character character;

    private void Start()
    {
        txt = uitxt.GetComponent<Text>();
    }

    public void SelectChar()
    {
        DataMgr.instance.currentCharacter = character;
        switch (character)
        {
            case Character.Hero_Knight:
                txt.text = "����";
                break;
            case Character.New1:
                txt.text = "��Ȯ��";
                break;
        }
    }
}
