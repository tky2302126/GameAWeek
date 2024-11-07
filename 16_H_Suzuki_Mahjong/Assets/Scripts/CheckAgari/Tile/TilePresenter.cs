
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

    public void Init(Suit _suit, Rank _rank) 
    {
        model.SetModel(_suit, _rank);

        int index =-1;
        if(_suit == Suit.Jihai)
        {
            index = ((int)_suit * 9) + ((int)_rank - 11); // Rank.East = 11
        }
        else
        {
            index = ((int)_suit * 9) + ((int)_rank - 1); // Rank.One = 1
        }

        view.SetImage(index);
    }
}