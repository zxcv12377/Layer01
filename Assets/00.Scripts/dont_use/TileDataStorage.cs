using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

//public class TileData
//{
//    public TileData(string id)
//    {
//        this.id = id;
//    }

//    public string id { get; set; }
//}
//public class TileDataStorage : DataStorageBase
//{
//    //public static int MAX_TILE_PROFICIENCY_LV = 8;
//    //private Dictionary<string, object> m_TileData = new Dictionary<string, object>();
//    private List<Dictionary<string, object>> m_TileData = new List<Dictionary<string, object>>();
//    public List<Dictionary<string, object>> tileData => m_TileData;

//    private Dictionary<string, TileData> s_tileData = new Dictionary<string, TileData>();
//    public Dictionary<string, TileData> v_tileDatas => s_tileData;

//    public override void LoadData()
//    {
//        foreach(var d in CSVReader.Read("NewTileData"))
//        {
//            tileData.Add((Dictionary<string, object>)d["ID"]);
//            //var test = d.Values.Count;
//            //v_tileDatas.Add()
//        }
//    }

//    //public bool TryGetTileData(string id, out TileData data)
//    //{
//    //    //return tileData.
//    //}
//    public override string GetLog()
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.AppendLine($"###### {GetType()} ######");

//        // 로그 내용 추가

//        return sb.ToString();
//    }
//}

[System.Serializable]
public class TileData
{
    [MiniCSV.CsvConstructor]
    public TileData(string id, string kind)
    {
        this.id = id;
        this.kind = kind;
    }

    public string id { get; set; }
    public string kind { get; set; }

}

public class TileDataStorage : DataStorageBase
{
    private Dictionary<string, TileData> m_TileDatas = new Dictionary<string, TileData>();
    public Dictionary<string, TileData> tileDatas => m_TileDatas;

    public override void LoadData()
    {
        MiniCSV_CsvParser.Deserialize<TileData>("NewTileData", (d) =>
        {
            tileDatas.Add(d.id, d);
        });
    }

    public bool TryGetTileData(string id, out TileData data)
    {
        return tileDatas.TryGetValue(id, out data);
    }

    public override string GetLog()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"■■■■■ {GetType()} ■■■■■");

        // 로그 내용 추가

        return sb.ToString();
    }
}

