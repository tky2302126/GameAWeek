using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data",menuName = "ScriptableObject/TileImage")]
public class TileImage : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();
}
