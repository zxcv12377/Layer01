using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButtonController : MonoBehaviour
{
    [SerializeField] private GameObject[] CharacterUI;
    [SerializeField] private GameObject uitxt;
    private Text txt;

    private void Start()
    {
        txt = uitxt.GetComponent<Text>();
    }
}
