using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(nameof(Test));
    }

    IEnumerator Test()
    {
        StartCoroutine(nameof(Test2));
        yield return new WaitForSeconds(1f);
        Debug.Log("첫번째");

    }

    IEnumerator Test2()
    {
        Debug.Log("두번째");
        yield return new WaitForSeconds(3f);
        Debug.Log("세번째");
    }
}
