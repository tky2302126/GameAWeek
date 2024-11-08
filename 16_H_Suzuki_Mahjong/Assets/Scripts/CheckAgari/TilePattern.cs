using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data",menuName ="ScriptableObject/CreateTilePattern")]

public class TilePattern : ScriptableObject
{
    [SerializeField]
    List<TileModel> models = new List<TileModel>();
}
