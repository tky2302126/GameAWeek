
using System;
using System.Collections.Generic;
using ShisenSho;
using UnityEngine;


/// <summary>
/// modelクラスは不要(sbyte idのみ)になった
/// </summary>
public class TilePresenter : MonoBehaviour
{
    // you may write here methods

    private sbyte id;
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