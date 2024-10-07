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

// IEquatable : �N���X�̓��������`����
public struct BoardPosition :IEquatable<BoardPosition> 
{
    public int row;
    public int col;

    public BoardPosition(int row, int col) 
    {
        this.row = row;
        this.col = col;
    }

    //IEquatable�ŕK�v
    //�ǂ̃v���p�e�B���r���邩��`����
    public bool Equals(BoardPosition other) 
    {
        return row == other.row && col == other.col;
    }

    // �^���m�̓������̔���ɕK�v
    public override bool Equals(object obj)
    {
        return obj is BoardPosition other && Equals(other);
    }

    // �n�b�V���l�ł̂���������Ƃ��ɂ���Ɨǂ�
    //
    public override int GetHashCode()
    {
        return HashCode.Combine(row, col);
    }

    // �������̒�`
    // == �� != ���`����
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

    //�A�j���[�V�����Đ��p�̔Ֆʂ̍\����
    //�ՖʂƓ����ʒu�ɖړI�n����͂���
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

    //���͂��ꂽ�Ֆʂ���ʂɔ��f����
    private IEnumerator myUpdate() 
    {
        //BoardView�N���X���Ȃ��Ƃ��X�V�ł��Ȃ�
        if(_boardView == null) 
        {
            yield break;
        }
        myUpdatePlaying = true;

        try 
        {
                var boardSpawn = new int[Board.GetLength(0),Board.GetLength(1)];
            //�ړ���Ƀu���b�N�𐶐�
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

            //�{�[�h�̃A�j���[�V�����ƃX�R�A�X�V
            if (BoardScore != null || BoardMove != null)
            {
                //�Đ��w��
                yield return _boardView.PlayUpdateAnimetion(Board, BoardMove,boardSpawn);
                //������
                BoardScore = null;
                BoardMove = null;
            }
            else
            {
                //�����ڂ�ύX����
                _boardView.Set(Board);
            }

            ForecastScore();

            //�Q�[���I�[�o�[����
            if(IsGameOver()== false) 
            {
                yield break;
                //try�̈��E�o����finally�ɍs��
            }

            //�Q�[���I�[�o�[����
            { 
                gameOverUI.SetActive(true);
                _inputActionAsset.Disable();
                yield return new WaitForSeconds(1f); 
                _sessionData.score = Score;
                GoToResult();
            
            }
            
        }
        //update�̏I���𖾎��I�Ɏ���
        finally
        {
            myUpdatePlaying =false;
        }


    }

    //�ړ�����->�ՖʂɐG��

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
    //�Ֆʂ�����������
    bool SlideBoard(InputType inputType) 
    {
        bool result = false;

        var addScore = 0;

        var moved = false;

        BoardScore = new int[_sessionData.Rows,_sessionData.Columns];

        InitBoardMove();


        //�l�n���͕ς���Ă��܂����߁A�R�s�[��n��
        var previousBoard = (int[,])Board.Clone();

        var previousScore = Score;

#if false
        //�]���̂������ƃA�j���[�V��������ɕs�s��
        {
            //�l�߂�
            closeUp = CloseUpCell(inputType);
            //����
            addScore = MergeCell(inputType);
            //�l�߂�
            if(addScore >0) { CloseUpCell(inputType); }
        
        }
#else
        // -> ��̏����ɂ܂Ƃ߂�
        {
            MoveCell(inputType,previousBoard, ref moved);

            foreach(var add in BoardScore) 
            {
                addScore += add;
            }
        }

        //�Ֆʂ��r���ē����Ȃ�false
        //�����ł������Ȃ�true


#endif
        if (addScore >0 ||  moved) 
        {
            //�A�j���[�V�������̓o�^
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

        //�����ɂ���ď�������ς���
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
                        //�l�߂�
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //�ړ������ɋl�߂�
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;
                            result = true;
                            //���W�̍X�V
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
                        //�l�߂�
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //�ړ������ɋl�߂�
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;
                            result = true;
                            //���W�̍X�V
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

        //�����ɂ���ď�������ς���
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
                        //����
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

                    //����
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

        //�����ɂ���ď�������ς���
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
                        //�l�߂�
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //�ړ������ɋl�߂�
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;
                            
                            //
                            BoardMove[row,col] = new BoardPosition(nextrow, nextcol);
                            
                            //���W�̍X�V
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //����
                        //�����ς݂łȂ�������
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
                        //�l�߂�
                        if (Board[nextrow, nextcol] == 0)
                        {
                            //�ړ������ɋl�߂�
                            Board[nextrow, nextcol] = value;
                            Board[crrRow, crrCol] = 0;

                            BoardMove[row, col] = new BoardPosition(nextrow, nextcol);
                            
                            //���W�̍X�V
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //����
                        //�����ς݂łȂ�������
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

    //BoardMove�̓o�^
    //���l������}�X�܂ŒT������
    //����Γo�^
    //�Ȃ���Β[�܂ňړ��Ƃ���
    // ����Ȃ��H
    private void CompareBoard(int[,] previousBoard,InputType input) 
    {
        var RowSize = Board.GetLength(0);
        var ColSize = Board.GetLength(1);

        BoardMove = new BoardPosition[RowSize, ColSize];

        int[] rowAxisMove = { -1, 1, 0, 0 };
        int[] colAxisMove = { 0, 0, -1, 1 };

        //�����ɂ���ď�������ς���
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
                        //���l���o��܂ŒT������
                        if (Board[nextrow, nextcol] == value)
                        {
                            discovered = true;
                            _boardMove = new BoardPosition(nextrow, nextcol);
                        }
                        else 
                        {
                            //���W�̍X�V
                            nextrow = nextrow + rowAxisMove[(int)input];
                            nextcol = nextcol + colAxisMove[(int)input];
                        }
                    }

                    //�Ȃ������Ƃ��͒[�܂ňړ�����
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
                        //���l���o��܂ŒT������
                        if (Board[nextrow, nextcol] == value)
                        {
                            discovered = true;
                            _boardMove = new BoardPosition(nextrow, nextcol);
                        }
                        else
                        {
                            //���W�̍X�V
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

    //��̃}�X��T���ēn��
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

    //�u���b�N�𐶐�����
    public void SpawnCell(BoardPosition position,int value) 
    {
        Board[position.row,position.col] = value;
    }

    //�Ֆʂ��������Ȃ������肷��
    public bool IsGameOver() 
    {
        bool result = true;

        //�E�A�������̒l�����l�Ȃ�A�Q�[���I�[�o�[�ɂȂ�Ȃ�
        for(int row = 0;row < Board.GetLength(0); row++) 
        {
            for(int col = 0; col < Board.GetLength(1); col++) 
            {
                if (Board[row, col] == 0) { result = false; break; }
                var value = Board[row, col];

                //nullreference �G���[�h�~
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

        //�����ɂ���ď�������ς���
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
                        //�l�߂�
                        if (board[nextrow, nextcol] == 0)
                        {
                            //�ړ������ɋl�߂�
                            board[nextrow, nextcol] = value;
                            board[crrRow, crrCol] = 0;

                            

                            //���W�̍X�V
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //����
                        //�����ς݂łȂ�������
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
                        //�l�߂�
                        if (board[nextrow, nextcol] == 0)
                        {
                            //�ړ������ɋl�߂�
                            board[nextrow, nextcol] = value;
                            board[crrRow, crrCol] = 0;

                            //���W�̍X�V
                            crrRow = nextrow;
                            crrCol = nextcol;
                            nextrow = crrRow + rowAxisMove[(int)input];
                            nextcol = crrCol + colAxisMove[(int)input];

                        }
                        //����
                        //�����ς݂łȂ�������
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

    //�X�R�A��\������
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
