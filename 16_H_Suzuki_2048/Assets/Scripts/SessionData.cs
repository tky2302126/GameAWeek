using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SessionData")]
public class SessionData : ScriptableObject
{
    public int score;
    public int BoardSize = 4;
    public int Rows = 4;
    public int Columns =4;
}
