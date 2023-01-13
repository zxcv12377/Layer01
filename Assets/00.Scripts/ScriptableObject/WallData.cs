using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall Data", menuName = "Scriptable Object/Wall Data", order = int.MaxValue)]
public class WallData : ScriptableObject
{
    [SerializeField] private string wallName;
    public string WallName { get { return wallName; } }
    [SerializeField] private int currenthp;
    public int currentHp { get { return currenthp; } }
    [SerializeField] private int maxhp;
    public int maxHp { get { return maxhp; } }
    [SerializeField] private int defence;
    public int Defence { get { return defence; } }
    [SerializeField] private Sprite sprite;
    public Sprite Sprite { get { return sprite; } }
}
