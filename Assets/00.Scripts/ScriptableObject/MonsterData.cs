using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Object/Monster Data", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField] private string monsterName;
    public string MonsterName { get { return monsterName; } }
    [SerializeField] private int currenthp;
    public int currentHp { get { return currenthp; } }
    [SerializeField] private int maxhp;
    public int maxHp { get { return maxhp; } }
}
