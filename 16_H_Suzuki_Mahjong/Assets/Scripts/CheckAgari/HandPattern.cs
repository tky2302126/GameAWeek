using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandPatternData", menuName = "ScriptableObject/HandPatternData", order = 1)]
public class HandPattern : ScriptableObject
{
    public List<int[]> handPatterns;
}