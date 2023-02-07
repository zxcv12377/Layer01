using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEditor;
using TMPro;
using System.Text;

public class MapEditorManager : BaseMonoSingleton<MapEditorManager>
{
    private string LOAD_KEY = "LOAD_KEY";
    int current_KEY = 0;
    //#if UNITY_EDITOR
    [Header("파일 저장 데이터")]
    //ublic TMP_Dropdown BaseMapData;
    public TMP_Dropdown loadMap;
    public TMP_InputField saveTxt;
    public Transform leftDown;
    public Transform rightUp;
    public RectTransform rtLeftDown;
    public RectTransform rtRightUp;
    public TMP_Dropdown tileDropdown;

    public Button TilePrefab;
    public Transform TileParent;
    //public SpriteCollection spriteCollection;

    // path
    private static string path_Default = "Assets/09.LevelData/Default";
    private static string path1L = "Assets/09.LevelData/Default";
    private static string path1L_Contains = "Assets/09.LevelData/Level1";


    List<string> mapList = new List<string>();
    public Toggle gridToggle;
    private void Awake()
    {
        if (PlayerPrefs.HasKey(LOAD_KEY) == false) current_KEY = 0;
        else current_KEY = PlayerPrefs.GetInt(LOAD_KEY);
    }
    void Start()
    {
        //var defaultlevel = Resources.LoadAll<>
        tileDropdown.ClearOptions();
        //foreach(var t in )
        mapList.Add("Tile");
        mapList.Add("Spike");
        mapList.Add("BackGround");
        tileDropdown.AddOptions(mapList);
        tileDropdown.onValueChanged.AddListener(delegate { setDropDown(tileDropdown.value); });
        TileButtonCreate();
        //setDropDown(current_KEY);
        //gridToggle.onValueChanged.AddListener(delegate { setGrid(gridToggle.isOn); });

    }

    // 맵 로드 드롭다운 컨트롤
    private void setDropDown(int key)
    {
        PlayerPrefs.SetInt(LOAD_KEY, key);
        switch (key)
        {
            case 0:
                print("current key value : " + key);
                break;
            case 1:
                print("current key value : " + key);
                break;
            case 2:
                print("current key value : " + key);
                break;
            case 3:
                print("current key value : " + key);
                break;
            default:
                break;

        }
    }

    public void TileButtonCreate()
    {
        foreach (var data in DataManager.tileDataStorage.tileDatas)
        {
            string[] n = data.Value.id.Split('_');
            if (string.Compare(n[0], "spike") == 0)
            {
                var pos = leftDown.localPosition;

                Button newObj = Instantiate(TilePrefab, TileParent.position, Quaternion.identity, TileParent);
                newObj.onClick.AddListener(() =>
                {
                });
                newObj.image.sprite = ResourceManager.GetSprite(data.Value.id);
                //UI Size
                var txt = newObj.GetComponentInChildren<Text>();
                //txt.text = data.Value.id;
                newObj.name = data.Value.id;
                newObj.gameObject.SetActive(true);
            }
        }
    }
}
