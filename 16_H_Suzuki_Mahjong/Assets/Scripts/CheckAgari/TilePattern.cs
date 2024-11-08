using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatternData",menuName ="ScriptableObject/CreateTilePattern")]

public class TilePattern : ScriptableObject
{
    public List<List<sbyte>> patterns;
}
