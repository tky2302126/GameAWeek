using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPresenter : MonoBehaviour
{
    private BoardModel model;
    public BoardModel Model => model;

    [SerializeField]
    private BoardView view;

    private void Start()
    {
        model = new BoardModel();
    }



}
