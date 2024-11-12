
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// modelクラスは不要(sbyte idのみ)になった
/// </summary>
public class TilePresenter : MonoBehaviour
{
    // you may write here methods

    private sbyte id; //0~33で制御
    public sbyte Id => id;

    [SerializeField]
    private TileView view;
    public  TileView View => view;


    public void Init(sbyte _id) 
    {
        id = _id;

        view.SetImage(_id);
    }
}