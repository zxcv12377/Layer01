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
    //public Tilemap tilemap;
    //public TileBase tilebase;

    public Button mstPrefab;
    public Transform monsterParent;
    // Start is called before the first frame update
    void Start()
    {
        //for(int i = -5; i < 5; i++) 타일맵에 타일 붙여넣는 코드
        //{
        //    for(int j = -5; j < 5; j++)
        //    {
        //        tilemap.SetTile(new Vector3Int(i, j, 0), tilebase);
        //    }
        //}

        //var obj = Resources.LoadAll<WallData>("Enemy");
        //Sprite sprite = ResourceManager

        //foreach (var t in obj)
        //{
        //    Debug.Log(t);
        //}
        Button newObj = Instantiate(mstPrefab, monsterParent.position, Quaternion.identity, monsterParent);
        //newObj.image.sprite = Resource.GetSprite(data.Value.id);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
