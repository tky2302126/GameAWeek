using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public enum InputType 
{
    Up, Down, Left, Right
}

// IEquatable : クラスの等価性を定義する
public struct BoardPosition :IEquatable<BoardPosition> 
{
    public int row;
    public int col;

    public BoardPosition(int row, int col) 
    {
        this.row = row;
        this.col = col;
    }

    //IEquatableで必要
    //どのプロパティを比較するか定義する
    public bool Equals(BoardPosition other) 
    {
        return row == other.row && col == other.col;
    }

    // 型同士の等価性の判定に必要
    public override bool Equals(object obj)
    {
        return obj is BoardPosition other && Equals(other);
    }

    // ハッシュ値でのやり取りをするときにあると良い
    //
    public override int GetHashCode()
    {
        return HashCode.Combine(row, col);
    }

    // 等価性の定義
    // == と != を定義する
    public static bool operator ==(BoardPosition left, BoardPosition right) 
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoardPosition left, BoardPosition right) 
    {
        return !left.Equals(right); 
    }


}

public struct BoardInfo
{

    public BoardInfo(int[,] board, int score)
    {
        _board = board;
        _score = score;
    }

    public int[,] _board { get; private set; }
    public int _score { get; private set; }
}   

public class MainScreen : MonoBehaviour,IMainScreen
{
    [SerializeField]
    private InputActionAsset _inputActionAsset;

    [SerializeField]
    private BoardView _boardView;

    [SerializeField]
    private GameObject _readyUI;

    [SerializeField]
    private GameObject gameOverUI;
    public int[,] Board { get; set; }

    public int Score { get; set; }

    public int[,] BoardScore { get; set; }

    public BoardPosition BoardPosition { get; set; }

    [SerializeField]
    private SessionData _sessionData;

    private List<BoardInfo> prevBoardInfos;

    //アニメーション再生用の盤面の構造体
    //盤面と同じ位置に目的地を入力する
    public BoardPosition[,] BoardMove;

    bool myUpdatePlaying;

    [SerializeField]
    private TextMeshProUGUI _forecastScoreTextUp;

    [SerializeField]
    private TextMeshProUGUI _forecastScoreTextDown;

    [SerializeField]
    private TextMeshProUGUI _forecastScoreTextLeft;

    [SerializeField]
    private TextMeshProUGUI _forecastScoreTextRight;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init() 
    {
        var row = _sessionData.Rows;
        var col = _sessionData.Columns;
        var initBoard = new int[row,col];
        initBoard[UnityEngine.Random.Range(0, row), UnityEngine.Random.Range(0, col)] = UnityEngine.Random.Range(0, 10) <= 0 ? 4 : 2;
        InitGame(initBoard, 0);

        myUpdatePlaying = false;
        //_inputActionAsset.Enable();
        _inputActionAsset.FindAction("Up").performed += RecieveInputUp;
        _inputActionAsset.FindAction("Right").performed += RecieveInputRight;
        _inputActionAsset.FindAction("Down").performed += RecieveInputDown;
        _inputActionAsset.FindAction("Left").performed += RecieveInputLeft;
        _inputActionAsset.FindAction("Undo").performed += RecieveInputUndo;

        _boardView.UpdateScore(Score);
        _boardView.Set(Board);

        InitBoardMove();

        _forecastScoreTextUp.SetText("0");
        _forecastScoreTextDown.SetText("0");
        _forecastScoreTextLeft.SetText("0");
        _forecastScoreTextRight.SetText("0");

        _readyUI.SetActive(true);
        yield return CountDown(3);
        _readyUI.SetActive(false);
        _inputActionAsset.Enable();
    }

    void OnDestroy() 
    {
        UnInit();
    }

    private void UnInit() 
    {
        
        _inputActionAsset.FindAction("Up").performed -= RecieveInputUp;
        _inputActionAsset.FindAction("Right").performed -= RecieveInputRight;
        _inputActionAsset.FindAction("Down").performed -= RecieveInputDown;
        _inputActionAsset.FindAction("Left").performed -= RecieveInputLeft;
        _inputActionAsset.FindAction("Undo").performed -= RecieveInputUndo;

        _inputActionAsset.Disable();
    }

    public void InitGame(int[,] board, int score) 
    {
        Board = board;
        Score = score;
        prevBoardInfos = new List<BoardInfo>();
    }

    public void GoToResult()
    {
        SceneManager.LoadScene("Result");
    }

    public IEnumerator CountDown(int second) 
    {
        yield return new WaitForSeconds(second);
    }

    //入力された盤面を画面に反映する
    private IEnumerator myUpdate() 
    {
        //BoardViewクラスがないとき更新できない
        if(_boardView == null) 
        {
            yield break;
        }
        myUpdatePlaying = true;

        try 
        {
                var boardSpawn = new int[Board.GetLength(0),Board.GetLength(1)];
            //移動後にブロックを生成
            {
                var positions = GetEmptyPositions();
                if(positions.Length > 0) 
                {
                    var position = positions[UnityEngine.Random.Range(0, positions.Length)];
                    var value = UnityEngine.Random.Range(0, 10) <= 0 ? 4 : 2;
                    boardSpawn[position.row,position.col] = value;
                    SpawnCell(position, value);
                }

                _boardView.UpdateScore(Score);
            }

            //ボードのアニメーションとスコア更新
            if (BoardScore != null || BoardMove != null)
            {
                //再生指示
                yield return _boardView.PlayUpdateAnimetion(Board, BoardMove,boardSpawn);
                //初期化
                BoardScore = null;
                BoardMove = null;
            }
            else
            {
                //見た目を変更する
                _boardView.Set(Board);
            }

            ForecastScore();

            //ゲームオーバー判定
            if(IsGameOver()== false) 
            {
                yield break;
                //try領域を脱出してfinallyに行く
            }

            //ゲームオーバー処理
            { 
                gameOverUI.SetActive(true);
                _inputActionAsset.Disable();
                yield return new WaitForSeconds(1f); 
                _sessionData.score = Score;
                GoToResult();
            
            }
            
        }
        //updateの終了を明示的に示す
        finally
        {
            myUpdatePlaying =false;
        }


    }

    //移動処理->盤面に触る

    private void RecieveInputUp(InputAction.CallbackContext context) 
    {
        if (myUpdatePlaying) { return; }
        if (SlideBoard(InputType.Up)) 
        {
            StartCoroutine(myUpdate());
        }

    }

    private void RecieveInputRight(InputAction.CallbackContext context)
    {
        if (myUpdatePlaying) { return; }
        if (SlideBoard(InputType.Right))
        {
            StartCoroutine(myUpdate());
        }
    }

    private void RecieveInputLeft(InputAction.CallbackContext context)
    {
        if (myUpdatePlaying) { return; }
        if (SlideBoard(InputType.Left))
        {
            StartCoroutine(myUpdate());
        }
    }

    private void RecieveInputDown(InputAction.CallbackContext context)
    {
        if (myUpdatePlaying) { return; }
        if (SlideBoard(InputType.Down))
        {
            StartCoroutine(myUpdate());
        }
    }
    
    private void RecieveInputUndo(InputAction.CallbackContext context) 
    {
        if(myUpdatePlaying) { return; }
        if (prevBoardInfos.Count > 0) 
        {
            var index = prevBoardInfos.Count - 1;

            Board = prevBoardInfos[index]._board;
            Score = prevBoardInfos[index]._score;

            _boardView.Set(Board);

            prevBoardInfos.RemoveAt(index);
            ForecastScore();
        }
        else 
        {
            Debug.LogWarning("the List is Empty!");
        }
    }

    private void InitBoardMove() 
    {
        if (BoardMove != null) return;

        var row = _sessionData.Rows;
        var col = _sessionData.Columns;

        BoardMove = new BoardPosition[row,col];
        

        for(int i=0;i<row;i++) 
        {
            for(int j = 0; j < col; j++) 
            {
                BoardMove[i, j] = new BoardPosition(-1, -1);        
            }
        }
    }
    //盤面が動くか判定
    bool SlideBoard(InputType inputType) 
    {
        bool result = false;

        var addScore = 0;

        var moved = false;

        BoardScore = new int[_sessionData.Rows,_sessionData.Columns];

        InitBoardMove();


        //値渡しは変わってしまうため、コピーを渡す
        var previousBoard = (int[,])Board.Clone();

        var previousScore = Score;

#if false
        //従来のやり方だとアニメーション制御に不都合
        {
            //詰めて
            closeUp = CloseUpCell(inputType);
            //合成
            addScore = MergeCell(inputType);
            //詰める
            if(addScore >0) { CloseUpCell(inputType); }
        
        }
#else
        // -> 一つの処理にまとめる
        {
            MoveCell(inputType,previousBoard, ref moved);

            foreach(var add in BoardScore) 
            {
                addScore += add;
            }
        }

        //盤面を比較して同じならfalse
        //少しでも同じならtrue


#endif
        if (addScore >0 ||  moved) 
        {
            //アニメーション情報の登録
            //CompareBoard(previousBoard, inputType);
            Score += addScore;    
            var _boardInfo = new BoardInfo(previousBoard,previousScore);
            prevBoardInfos.Add(_boardInfo);
            result = true;
        }
        

        return result;
    }

    public bool CloseUpCell(InputType inputType) 
    {
        bool result = false;

        int[] rowAxisMove ={-1,1,0,0 };
        int[] colAxisMove ={0,0,-1,1 };

        //方向によって処理順を変える
        if(inputType == InputType.Left|| inputType == InputType.Up) 
        {
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var value = Board[row, col];
                    if (value == 0) { continue; }


                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)inputType];
                    var nextcol = col + colAxisMove[(int)inputType];

                    while(nextrow >= 0 && nextcol >= 0)
                    {
                        //詰める
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //移動方向に詰める
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;
                            result = true;
                            //座標の更新
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)inputType];
                            nextcol = crrCol + colAxisMove[(int)inputType];
                        }
                        else { break; }
                    }

                }
            }
        }

        if(inputType == InputType.Right|| inputType == InputType.Down) 
        {
            for (int row = Board.GetLength(0) - 1; row  >= 0; row--)
            {
                for (int col = Board.GetLength(1)-1; col >= 0; col--)
                {
                    var value = Board[row, col];
                    if (value == 0) { continue; }


                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)inputType];
                    var nextcol = col + colAxisMove[(int)inputType];

                    while
                        (
                        nextrow < Board.GetLength(0) &&
                        nextcol < Board.GetLength(1)
                        )
                    {
                        //詰める
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //移動方向に詰める
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;
                            result = true;
                            //座標の更新
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)inputType];
                            nextcol = crrCol + colAxisMove[(int)inputType];
                        }
                        else { break; }
                    }

                }
            }
        }
            


        return result;
    }

    public int MergeCell(InputType inputType)
    {
        int result = 0;

        int[] rowAxisMove = { -1, 1, 0, 0 };
        int[] colAxisMove = { 0, 0, -1, 1 };

        //方向によって処理順を変える
        if (inputType == InputType.Left || inputType == InputType.Up)
        {
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var value = Board[row, col];
                    if (value == 0) { continue; }

                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)inputType];
                    var nextcol = col + colAxisMove[(int)inputType];


                    if(nextcol >=0 &&nextrow >= 0) 
                    {
                        //合成
                        if (Board[nextrow, nextcol] == value)
                        {
                            Board[nextrow, nextcol] += value;
                            Board[crrRow, crrCol] = 0;
                            result += Board[nextrow,nextcol];                              
                        }
                    }
                }
            }
        }

        if (inputType == InputType.Right || inputType == InputType.Down)
        {
            for (int row = Board.GetLength(0) - 1; row >= 0; row--)
            {
                for (int col = Board.GetLength(1) - 1; col >= 0; col--)
                {
                    var value = Board[row, col];
                    if (value == 0) { continue; }

                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)inputType];
                    var nextcol = col + colAxisMove[(int)inputType];

                    //合成
                    if(nextrow < Board.GetLength(0) &&
                        nextcol < Board.GetLength(1)) 
                    {
                        if (Board[nextrow, nextcol] == value)
                        {
                            Board[nextrow, nextcol] += value;
                            Board[crrRow, crrCol] = 0;
                            result += Board[nextrow, nextcol];
                        }
                    }
                }
            }
        }

        return result;
    }

    private void MoveCell(InputType input, int[,] previousBoard , ref bool moved) 
    {
        

        int[] rowAxisMove = { -1, 1, 0, 0 };
        int[] colAxisMove = { 0, 0, -1, 1 };

        //方向によって処理順を変える
        if (input == InputType.Left || input == InputType.Up)
        {
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var value = Board[row, col];
                    if (value == 0) { continue; }


                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)input];
                    var nextcol = col + colAxisMove[(int)input];

                    while (nextrow >= 0 && nextcol >= 0)
                    {
                        //詰める
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //移動方向に詰める
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;
                            
                            //
                            BoardMove[row,col] = new BoardPosition(nextrow, nextcol);
                            
                            //座標の更新
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //合成
                        //合成済みでないか判定
                        else if (Board[nextrow,nextcol] == value && BoardScore[nextrow,nextcol] == 0) 
                        {
                            Board[nextrow, nextcol] += value;
                            Board[crrRow,crrCol] = 0;

                            BoardMove[row, col] = new BoardPosition(nextrow , nextcol);

                            BoardScore[nextrow ,nextcol] += Board[nextrow,nextcol];
                            break;
                        }
                        else 
                        {
                            break;
                        }
                        
                    }

                }
            }
        }

        if (input == InputType.Right || input == InputType.Down)
        {
            for (int row = Board.GetLength(0) - 1; row >= 0; row--)
            {
                for (int col = Board.GetLength(1) - 1; col >= 0; col--)
                {
                    var value = Board[row, col];
                    if (value == 0) { continue; }


                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)input];
                    var nextcol = col + colAxisMove[(int)input];

                    while
                        (
                        nextrow < Board.GetLength(0) &&
                        nextcol < Board.GetLength(1)
                        )
                    {
                        //詰める
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //移動方向に詰める
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;

                            BoardMove[row, col] = new BoardPosition(nextrow, nextcol);
                            
                            //座標の更新
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //合成
                        //合成済みでないか判定
                        else if (Board[nextrow, nextcol] == value && BoardScore[nextrow, nextcol] == 0)
                        {
                            Board[nextrow, nextcol] += value;
                            Board[crrRow, crrCol] = 0;

                            BoardMove[row, col] = new BoardPosition(nextrow, nextcol);

                            BoardScore[nextrow, nextcol] += Board[nextrow, nextcol];
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
            }
        }

        moved = CheckMoved(previousBoard);
    }


    private bool CheckMoved(int[,] previousBoard) 
    {
        bool result = false;
        var row = _sessionData.Rows;
        var col = _sessionData.Columns;

        for( var i = 0;i<row; i++) 
        {
            for( var j = 0; j <col; j++) 
            {
                if( previousBoard[i,j] != Board[i,j]) 
                {
                    result = true; break;
                }
            }
        }
        return result;
    }

    //BoardMoveの登録
    //同値があるマスまで探索する
    //あれば登録
    //なければ端まで移動とする
    // いらない？
    private void CompareBoard(int[,] previousBoard,InputType input) 
    {
        var RowSize = Board.GetLength(0);
        var ColSize = Board.GetLength(1);

        BoardMove = new BoardPosition[RowSize, ColSize];

        int[] rowAxisMove = { -1, 1, 0, 0 };
        int[] colAxisMove = { 0, 0, -1, 1 };

        //方向によって処理順を変える
        if (input == InputType.Left || input == InputType.Up)
        {
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    if (previousBoard[row,col] ==0) { BoardMove[row, col] = new BoardPosition(-1,-1); continue; }
                    var value = Board[row,col];

                    BoardPosition _boardMove = new BoardPosition();
                    var discovered = false;
                    var nextrow = row + rowAxisMove[(int)input];
                    var nextcol = col + colAxisMove[(int)input];

                    while (nextrow >= 0 && nextcol >= 0)
                    {
                        //同値が出るまで探索する
                        if (Board[nextrow, nextcol] == value)
                        {
                            discovered = true;
                            _boardMove = new BoardPosition(nextrow, nextcol);
                        }
                        else 
                        {
                            //座標の更新
                            nextrow = nextrow + rowAxisMove[(int)input];
                            nextcol = nextcol + colAxisMove[(int)input];
                        }
                    }

                    //なかったときは端まで移動する
                    if(!discovered) 
                    {
                       if(nextcol < 0) { nextcol = 0; }
                       if(nextrow < 0) { nextrow = 0; }
                        _boardMove = new BoardPosition(nextrow, nextcol);
                    }

                    BoardMove[row,col] = _boardMove;

                }
            }
        }

        if (input == InputType.Right || input == InputType.Down)
        {
            for (int row = Board.GetLength(0) - 1; row >= 0; row--)
            {
                for (int col = Board.GetLength(1) - 1; col >= 0; col--)
                {
                    if (previousBoard[row, col] == 0) { BoardMove[row, col] = new BoardPosition(-1, -1); continue; }
                    var value = Board[row, col];

                    BoardPosition _boardMove = new BoardPosition();
                    var discovered = false;
                    var nextrow = row + rowAxisMove[(int)input];
                    var nextcol = col + colAxisMove[(int)input];

                    while 
                        (
                        nextrow < Board.GetLength(0) &&
                        nextcol < Board.GetLength(1)
                        )
                    {
                        //同値が出るまで探索する
                        if (Board[nextrow, nextcol] == value)
                        {
                            discovered = true;
                            _boardMove = new BoardPosition(nextrow, nextcol);
                        }
                        else
                        {
                            //座標の更新
                            nextrow = nextrow + rowAxisMove[(int)input];
                            nextcol = nextcol + colAxisMove[(int)input];
                        }
                    }

                    if (!discovered) 
                    {
                        if(nextrow >= Board.GetLength(0)) { nextrow = Board.GetLength(0) - 1; }
                        if(nextcol >= Board.GetLength(1)) { nextcol = Board.GetLength(1) - 1; }
                        _boardMove = new BoardPosition(nextrow, nextcol);
                    }

                    BoardMove[row, col] = _boardMove;

                }
            }
        }
    }

    //空のマスを探して渡す
    public BoardPosition[] GetEmptyPositions() 
    {
        List<BoardPosition> emptyPositions = new List<BoardPosition>();

        for(int row = 0; row < Board.GetLength(0); row++) 
        {
            for(int col =0; col < Board.GetLength(1); col++) 
            {
                if (Board[row, col] == 0) 
                {
                    BoardPosition position = new BoardPosition(row,col);
                    emptyPositions.Add(position);
                }
            }
        }

        BoardPosition[] boardPositions = new BoardPosition[emptyPositions.Count];

        for(int i=0; i < emptyPositions.Count; ++i) 
        {
            boardPositions[i] = emptyPositions[i];
        }



        return boardPositions;
    }

    //ブロックを生成する
    public void SpawnCell(BoardPosition position,int value) 
    {
        Board[position.row,position.col] = value;
    }

    //盤面が動かせないか判定する
    public bool IsGameOver() 
    {
        bool result = true;

        //右、下方向の値が同値なら、ゲームオーバーにならない
        for(int row = 0;row < Board.GetLength(0); row++) 
        {
            for(int col = 0; col < Board.GetLength(1); col++) 
            {
                if (Board[row, col] == 0) { result = false; break; }
                var value = Board[row, col];

                //nullreference エラー防止
                if (col < 3)
                {
                    if (Board[row, col + 1] == value) { result = false; break; }
                }
                if (row < 3)
                {
                    if (Board[row + 1, col] == value) { result = false; break; }
                }
            }
        }


        return result;
    }

    private int ComputeScore(InputType input, int[,] _board) 
    {
        int result = 0;

        var board = (int[,])_board.Clone();

        var boardScore = new int[Board.GetLength(0),Board.GetLength(1)];

        int[] rowAxisMove = { -1, 1, 0, 0 };
        int[] colAxisMove = { 0, 0, -1, 1 };

        //方向によって処理順を変える
        if (input == InputType.Left || input == InputType.Up)
        {
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    var value = board[row, col];
                    if (value == 0) { continue; }


                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)input];
                    var nextcol = col + colAxisMove[(int)input];

                    while (nextrow >= 0 && nextcol >= 0)
                    {
                        //詰める
                        if (board[nextrow, nextcol] == 0)
                        {
                            //移動方向に詰める
                            board[nextrow, nextcol] = value;
                            board[crrRow, crrCol] = 0;

                            

                            //座標の更新
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //合成
                        //合成済みでないか判定
                        else if (board[nextrow, nextcol] == value && boardScore[nextrow, nextcol] == 0)
                        {
                            board[nextrow, nextcol] += value;
                            board[crrRow, crrCol] = 0;

                            boardScore[nextrow,nextcol] += board[nextrow, nextcol];
                            break;
                        }
                        else
                        {
                            break;
                        }

                    }

                }
            }
        }

        if (input == InputType.Right || input == InputType.Down)
        {
            for (int row = Board.GetLength(0) - 1; row >= 0; row--)
            {
                for (int col = Board.GetLength(1) - 1; col >= 0; col--)
                {
                    var value = board[row, col];
                    if (value == 0) { continue; }


                    var crrRow = row;
                    var crrCol = col;
                    var nextrow = row + rowAxisMove[(int)input];
                    var nextcol = col + colAxisMove[(int)input];

                    while
                        (
                        nextrow < Board.GetLength(0) &&
                        nextcol < Board.GetLength(1)
                        )
                    {
                        //詰める
                        if (board[nextrow, nextcol] == 0)
                        {
                            //移動方向に詰める
                            board[nextrow, nextcol] = value;
                            board[crrRow, crrCol] = 0;

                            //座標の更新
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //合成
                        //合成済みでないか判定
                        else if (board[nextrow, nextcol] == value && boardScore[nextrow, nextcol] == 0)
                        {
                            board[nextrow, nextcol] += value;
                            board[crrRow, crrCol] = 0;

                            boardScore[nextrow, nextcol] += board[nextrow, nextcol];
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
            }
        }

        foreach( var addscore in boardScore) 
        {
            result += addscore;
        }

        return result;
    }

    //スコアを予測する
    private void ForecastScore() 
    {
        
        for(int i = 0; i < 4; i++) 
        {
            var addScore = ComputeScore((InputType)i,Board);

            switch (i) 
            {
                case 0:
                    _forecastScoreTextUp.SetText(addScore.ToString()); 
                    break;

                case 1:
                    _forecastScoreTextDown.SetText(addScore.ToString()); 
                    break;

                case 2:
                    _forecastScoreTextLeft.SetText(addScore.ToString());
                    break;

                case 3:
                    _forecastScoreTextRight.SetText(addScore.ToString());
                    break;

            }
        }


    }

}
