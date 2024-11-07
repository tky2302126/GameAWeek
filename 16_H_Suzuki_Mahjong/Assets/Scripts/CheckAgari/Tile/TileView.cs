
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    // you may write here view contents
    [SerializeField]
    private TileImage images;
    [SerializeField]
    private Image image;

    public void SetImage(int index)
    {
        if (index ==-1)
        {
            Debug.LogAssertion("index is Wrong!");
            return;
        }
        image.sprite = images.sprites[index];
    }
}