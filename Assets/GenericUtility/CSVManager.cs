using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

// ---------------------------------------------
//
// v1.2 ('20.09.12)
//  - CsvHelper 사용 시 IL2CPP로 빌드한 경우 에러가 발생
//  - MiniCSV 스크립트로 대체 사용 (MiniCSV_CsvParser)
//
// v1.1 ('20.05.04)
//  - ParseCsv 함수에서 Line Split 할 때 비어있는 라인이 각 라인사이에 추가되는 부분 제거
//  - Custom CsvParser과 CsvHelper_CsvParser 분리
//
// ---------------------------------------------


/// <summary>
/// <para>CsvHelper 라이브러리 필요</para>
/// <para>홈페이지 : https://joshclose.github.io/CsvHelper/ </para>
/// </summary>
public static class CsvHelper_CsvParser
{
    /*

        [   사용 예시   ]
        * 클래스 또는 구조체의 변수는 무조건 { get; set; } 프로퍼티를 사용해야한다.
        * 프로퍼티의 이름과 csv 헤더의 이름이 동일해야 한다. (대소문자 까지 같아야함)
        * csv파일에 없는 변수를 넣고 싶을 땐 일반 변수로 넣으면 됨
        
        public struct UnitData
        {
            public string id { get; set; } // csv 파일에 잇는 값은 헤더값과 변수명 동일 (대소문자까지)
            public int level; // 일반 변수로 넣으면 csv 파일에 없어도 변환 가능
            public int unitType { get; set; }
            public EUnitClass unitClass { get; set; }
            public int cost { get; set; }
            public int maxHp { get; set; }
            public int maxShield { get; set; }
            public float defense { get; set; }
            public int attackDamage { get; set; }
            public float attackSpeed { get; set; }
            public float attackRange { get; set; }
            public float criticalPerc { get; set; }
            public float criticalDamage { get; set; }
            public float moveSpeed { get; set; }
            public float resistance { get; set; }
        }

        CsvHelper_CsvParser.Deserialize<UnitData>("UnitData", (datas) =>
        {
            foreach (var d in datas)
            {
                unitDatas.Add(d.id, d);
            }
        });

    */

    public static void Deserialize<T>(string path, UnityAction<IEnumerable<T>> onDesrialized)
    {
        TextAsset csvTextAsset = Resources.Load(path) as TextAsset;
        using (System.IO.TextReader sr = new System.IO.StringReader(csvTextAsset.text))
        {
            CsvHelper.CsvReader reader = new CsvHelper.CsvReader(sr, System.Globalization.CultureInfo.InvariantCulture);
            onDesrialized?.Invoke(reader.GetRecords<T>());
        }
    }

    public static List<T> Deserialize<T>(string path)
    {
        TextAsset csvTextAsset = Resources.Load(path) as TextAsset;
        using (System.IO.TextReader sr = new System.IO.StringReader(csvTextAsset.text))
        {
            CsvHelper.CsvReader reader = new CsvHelper.CsvReader(sr, System.Globalization.CultureInfo.InvariantCulture);
            List<T> datas = new List<T>();
            foreach(var d in reader.GetRecords<T>())
            {
                datas.Add(d);
            }
            return datas;
        }
    }
}

/// <summary>
/// 이 클래스만 있어도 사용 가능
/// </summary>
public static class Custom_CsvParser
{
    /* 
    
        [   사용 예시   ]

        public struct UnitData
        {
            public string id { get; set; }
            public int unitType { get; set; }
            public EUnitClass unitClass { get; set; }
            public int cost { get; set; }
            public int maxHp { get; set; }
            public int maxShield { get; set; }
            public float defense { get; set; }
            public int attackDamage { get; set; }
            public float attackSpeed { get; set; }
            public float attackRange { get; set; }
            public float criticalPerc { get; set; }
            public float criticalDamage { get; set; }
            public float moveSpeed { get; set; }
            public float resistance { get; set; }
        }

        var datas = Custom_CsvParser.ReadAndParse("UnitData");
        for (int i = 0; i < datas.Count; ++i)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var data in datas[i])
            {
                sb.AppendLine(data + "/");
            }
            Debug.Log(sb.ToString());

            var d = datas[i];
            if (d != null && d.Count > 12)
            {
                UnitData data = new UnitData()
                {
                    id = d[0],
                    unitClass = (EUnitClass)int.Parse(d[2]),
                    cost = int.Parse(d[3]),
                    maxHp = int.Parse(d[4]),
                    maxShield = int.Parse(d[5]),
                    defense = float.Parse(d[6]),
                    attackDamage = int.Parse(d[7]),
                    attackSpeed = float.Parse(d[8]),
                    attackRange = float.Parse(d[9]),
                    criticalPerc = float.Parse(d[10]),
                    criticalDamage = float.Parse(d[11]),
                    moveSpeed = float.Parse(d[12]),
                    resistance = float.Parse(d[13]),
                };
                unitDatas.Add(data.id, data);
            }
        }

    */

    private const string SPLIT_RE = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
    private const string LINE_SPLIT_RE = "[\n\r]";

    private static string ReadString(string path)
    {
        TextAsset csvTextAsset = Resources.Load(path) as TextAsset;
        return csvTextAsset.text;
    }

    public static List<List<string>> ParseCsv(string str)
    {
        string[] dataLines = Regex.Split(str, LINE_SPLIT_RE);
        List<List<string>> Datas = new List<List<string>>(); //첫째줄은 항목명으로 사용하니 0은 빈 리스트, 1부터 시작함
        int num = 1;
        string[] tokens;
        List<string> list;
        Datas.Add(new List<string>());

        while (num < dataLines.Length)
        {
            tokens = Regex.Split(dataLines[num], SPLIT_RE);
            if(tokens != null)
            {
                list = new List<string>(tokens);
                foreach(var t in tokens)
                {
                    Debug.Log(t);
                }
                Datas.Add(list);
            }
            num++;
        }
        return Datas;
    }

    public static List<List<string>> ReadAndParse(string path)
    {
        return ParseCsv(ReadString(path));
    }
}

public static class MiniCSV_CsvParser
{
    public static void Deserialize<T>(string path, UnityAction<T> onDesrialized) where T : class
    {
        TextAsset csvTextAsset = Resources.Load(path) as TextAsset;
        using (System.IO.TextReader sr = new System.IO.StringReader(csvTextAsset.text))
        {
            MiniCSV.CsvDeserializer<T> deserializer = new MiniCSV.CsvDeserializer<T>(new MiniCSV.CsvParser(sr));
            while (deserializer.TryProduce(out T d))
            {
                onDesrialized(d);
            }
        }
    }
}