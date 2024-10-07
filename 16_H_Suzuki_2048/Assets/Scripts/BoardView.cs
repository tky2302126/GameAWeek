using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public IEnumerator PlayUpdateAnimetion(int[,] board, BoardPosition[,] boardMove, int[,] boardSpawn) 
    {
        for(int row =0;row< _sessionData.Rows;  row++) 
        {
            for(int col =0;col< _sessionData.Columns; col++) 
            {
                    var boardPosition = new BoardPosition(row, col);
                //移動
                //入力されていなければ飛ばす
                {
                    if (boardMove[row,col].row ==-1 || boardMove[row, col].col == -1) { continue; }
                    var view = cellViews.FirstOrDefault(x => x.CurrentBoardPosition == boardPosition && x.IsShow);
                    if(view != null) 
                    {
                        var destinaton = boardMove[row, col];
                        StartCoroutine(view.PlayMoveCoroutine(boardMove[row,col], board[destinaton.row,destinaton.col]));
                    }
                }

                // 生成
                // うまくいってない
                if (boardSpawn[row, col] > 0)
                {
                    var view = cellViews.FirstOrDefault(cell => cell.IsShow == false);
                    if (view != null)
                    {
                        StartCoroutine(view.PlaySpawnCoroutine(boardPosition, boardSpawn[row, col]));
                    }
                }

            }
        }

        yield return new WaitForSeconds(_duration);

        // 重複セルを非表示
        for (var row = 0; row < _sessionData.Rows; row++)
        {
            for (var column = 0; column < _sessionData.Columns; column++)
            {
                var boardPosition = new BoardPosition(row, column);

                var cells = cellViews.Where(x => x.CurrentBoardPosition == boardPosition && x.IsShow).ToList();
                for (var i = 1; i < cells.Count; i++)
                {
                    cells[i].Hide();
                }
            }
        }

        Set(board);

        //スコアエフェクト再生
    }
    
}
