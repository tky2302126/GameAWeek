using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class MainScreen : MonoBehaviour,IMainScreen
{
    [SerializeField]
    private InputActionAsset _inputActionAsset;

    [SerializeField]
    private GameObject _readyUI;
    public int[,] Board { get; set; }

    public int Score { get; set; }

    bool myUpdatePlaying;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private IEnumerator Init() 
    {
        var BoardSize =SessionData.instance.BoardSize;
        var initBoard = new int[BoardSize,BoardSize];
        initBoard[Random.Range(0, BoardSize), Random.Range(0, BoardSize)] = Random.Range(0, 10) <= 0 ? 4 : 2;
        InitGame(initBoard, 0);

        myUpdatePlaying = false;
        _inputActionAsset.Enable();
        _inputActionAsset.FindAction("Up").performed += RecieveInputUp;
        _inputActionAsset.FindAction("Right").performed += RecieveInputRight;
        _inputActionAsset.FindAction("Down").performed += RecieveInputDown;
        _inputActionAsset.FindAction("Left").performed += RecieveInputLeft;



        _readyUI.SetActive(true);
        yield return CountDown(3);
        _readyUI.SetActive(false);
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

    private IEnumerator myUpdate() 
    {
        //
        myUpdatePlaying = true;

        //ボードのアニメーションとスコア更新
        { 
        
            yield return null; //ゲームオーバー時の処理
            SessionData.instance.score = Score;
            GoToResult();
        
        }

        {
            myUpdatePlaying =false;
        }


    }

    private void RecieveInputUp(InputAction.CallbackContext context) 
    {

    }

    private void RecieveInputRight(InputAction.CallbackContext context)
    {

    }

    private void RecieveInputLeft(InputAction.CallbackContext context)
    {

    }

    private void RecieveInputDown(InputAction.CallbackContext context)
    {

    }


}
