
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// model�N���X�͕s�v(sbyte id�̂�)�ɂȂ���
/// </summary>
public class TilePresenter : MonoBehaviour
{
    // you may write here methods

    private sbyte id; //0~33�Ő���
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