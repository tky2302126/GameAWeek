
using System;
using System.Collections.Generic;
using ShisenSho;
using UnityEngine;

public class TilePresenter : MonoBehaviour
{
    // you may write here methods

    private TileModel model;

    [SerializeField]
    private TileView view;
    public  TileView View => view;

    private void Start()
    {
        model = new TileModel();
    }

    public void Init(int _id) 
    {
        if (model == null) { model = new TileModel(); }
        model.SetModel(_id);

        view.SetImage(_id);
    }
}