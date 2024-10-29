using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data",menuName = "ScriptableObject/TileImage")]
public class TileImage : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();
}
