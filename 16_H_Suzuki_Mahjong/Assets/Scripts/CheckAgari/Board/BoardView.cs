using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField]
    private RectTransform tilesHolder;

    public  RectTransform TilesHolder => tilesHolder;
}
