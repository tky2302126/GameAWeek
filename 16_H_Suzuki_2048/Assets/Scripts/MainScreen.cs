using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public enum InputType 
{
    Up, Down, Left, Right
}

public struct BoardPosition 
{
    public int row;
    public int col;

    public BoardPosition(int row, int col) 
    {
        this.row = row;
        this.col = col;
    }
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

    [SerializeField]
    private SessionData _sessionData;




    //�A�j���[�V�����Đ��p�̔Ֆʂ̍\����
    public BoardPosition[,] BoardMove;

    bool myUpdatePlaying;

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

        _boardView.UpdateScore(Score);
        _boardView.Set(Board);

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

        _inputActionAsset.Disable();
    }

    public void InitGame(int[,] board, int score) 
    {
        Board = board;
        Score = score;
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
            //�ړ���Ƀu���b�N�𐶐�
            {
                var boardSpawn = new int[Board.GetLength(0),Board.GetLength(1)];
                var positins = GetEmptyPositions();
                if(positins.Length > 0) 
                {
                    var position = positins[UnityEngine.Random.Range(0, positins.Length)];
                    var value = UnityEngine.Random.Range(0, 10) <= 0 ? 4 : 2;
                    boardSpawn[position.row,position.col] = value;
                    SpawnCell(position, value);
                }

                _boardView.UpdateScore(Score);
            }

            //�{�[�h�̃A�j���[�V�����ƃX�R�A�X�V
            {

            }
            //else
            {
                _boardView.Set(Board);
            }
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
    //�Ֆʂ�����������
    bool SlideBoard(InputType inputType) 
    {
        bool result = false;

        //�l�߂�
        var closeUp = CloseUpBoard(inputType);
        //����
        var addScore = 0;
        addScore = SynthesisBoard(inputType);
        //�l�߂�
        if(addScore >0) { CloseUpBoard(inputType); }

        if(addScore >0 ||  closeUp) 
        {
            Score += addScore;    
            result = true;
        }
        

        return result;
    }

    public bool CloseUpBoard(InputType inputType) 
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

    public int SynthesisBoard(InputType inputType)
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
}
