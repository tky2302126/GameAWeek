using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField]
    private GameObject _cellPrefab;

    [SerializeField]
    private GameObject _cellContainer;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    private List<CellView> cellViews;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private SessionData _sessionData;

   

    
    void Start()
    {
        cellViews = new List<CellView>();
        //
        var row = _sessionData.Rows;
        var col = _sessionData.Columns;
        for (int i=0; i< row* col; i++) 
        {
            cellViews.Add(Instantiate(_cellPrefab,_cellContainer.transform).GetComponent<CellView>());
        }
    }


    public void UpdateScore(int score) 
    {
        _scoreText.SetText(score.ToString());
    }

    //ゲームオーバー演出(アニメーション)

    public void Set(int[,] Board) 
    {
        for(int i=0;i< _sessionData.Rows* _sessionData.Columns; i++) 
        {
            var row = i % _sessionData.Rows;
            var col = i / _sessionData.Rows;

            var value = Board[row, col];
            if(value == 0) 
            {
                cellViews[i].Hide();
            }
            else 
            {
                cellViews[i].Set(new BoardPosition(row, col), Board[row,col]);
            }
        }
    }

    //移動アニメーション・スコア更新アニメーション

    
}
