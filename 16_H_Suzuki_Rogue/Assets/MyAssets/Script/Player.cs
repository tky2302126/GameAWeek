using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.Windows;
using System;
using Random = UnityEngine.Random;

public enum InputType
{
    Up, Down, Left, Right
}

public class Player : absMove
{
    
    public float restartLevelDelay = 1f;
    public int pointPerFood = 10;
    public int pointPerSoda = 30;
    public int wallDamage = 1;
    public TextMeshProUGUI _foodText;

    [SerializeField]
    private InputActionAsset _input;
    private Animator _anim;
    private int food;
    private bool enterExit = false;

    private int footSteps = 0;

    readonly int[] rowAxisMove =  { 1, -1, 0, 0 };

    readonly int[] columnAxisMove = {0,0,-1,1 };

    readonly int INF = (int)Math.Exp(5);

    private bool turnLeft = false;

    protected override void Start()
    {
       _anim = GetComponent<Animator>();

        //scriptableObjectに置き換えできそう
        food = GameManager.instance.playerFoodPoints;
#if UNITY_EDITOR
        // food = 10;
#endif

        SetupFoodText();

        _input.FindAction("Up").performed += RecieveInputUp;
        _input.FindAction("Down").performed += RecieveInputDown;
        _input.FindAction("Left").performed += RecieveInputLeft;
        _input.FindAction("Right").performed += RecieveInputRight;
        _input.Enable();

        base.Start();
    }

    private void OnDisable()
    {
        _input.FindAction("Up").performed -= RecieveInputUp;
        _input.FindAction("Down").performed -= RecieveInputDown;
        _input.FindAction("Left").performed -= RecieveInputLeft;
        _input.FindAction("Right").performed -= RecieveInputRight;
        _input.Disable();

    }

    public void SetupFoodText() 
    {
        var text = "Food : " + food;
        if (_foodText == null)
        {
            _foodText = GameObject.Find("FoodText").GetComponent<TextMeshProUGUI>();
        }
        _foodText.SetText(text);
    }

    //myUpdateにして入力を受けたら呼び出すようにする
    //2048から持ってくるとよさそう
    void myUpdate(InputType input)
    {
        if (!GameManager.instance.playersTurn) { return; }

        AttemptMove<Wall>( columnAxisMove[(int)input], rowAxisMove[(int)input]);

        GameManager.instance.playersTurn = false;
    }

    //! foodを消費する前に移動可能か調べる
   public override void AttemptMove<T>(int xDir,int yDir) 
    {
        //動ける場合、foodを消費する
        food--;
        if (_foodText == null)
        {
            _foodText = GameObject.Find("FoodText").GetComponent<TextMeshProUGUI>();
        }
        var text = "Food : " + food;

        _foodText.SetText(text);

        base.AttemptMove<T>(xDir,yDir);

        RaycastHit2D hit;

        // SE再生
        //if(Move(xDir,yDir,out hit)) 
        {
            footSteps++;
            var se = footSteps % 2 == 0 ? SE.FootStep1 : SE.FootStep2;
            SoundManager.Instance.PlaySE(se);
            Debug.Log("Walk SE Played");
        }

        CheckIsGameOver();

        GameManager.instance.playersTurn =false;
    }

    public override void OnCantMove<T>(T component) 
    {
        Wall hitWall = component as Wall;

        hitWall.DamageWall(wallDamage);

        _anim.SetTrigger("attack");

        var se = Random.value >= 0.5 ? SE.Attack1 : SE.Attack2;

        SoundManager.Instance.PlaySE(se);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit") 
        {
            StartCoroutine(Restart(restartLevelDelay));

            enabled = false;

            SoundManager.Instance.PlaySE(SE.Stair);
        }

        else if(other.tag == "Food") 
        {
            food += pointPerFood;

            var text = "+" + pointPerFood + "Food : " + food;

            _foodText.SetText(text);

            other.gameObject.SetActive(false);

            var se = Random.value <= 5.0? SE.Fruit1 : SE.Fruit2;

            SoundManager.Instance.PlaySE(se);
        }
        else if(other.tag == "Soda") 
        {
            food += pointPerSoda;

            var text = "+" + pointPerSoda + "Food : " + food;

            _foodText.SetText(text);

            other.gameObject.SetActive(false);

            var se = Random.value <= 5.0 ? SE.Soda1 : SE.Soda2;

            SoundManager.Instance.PlaySE(se);
        }
        
    }

    // levelとfoodの更新
    private IEnumerator Restart(float delayTime)
    {
        //次のフロアへフードの値を引き継ぐ
        GameManager.instance.playerFoodPoints = food;

        // なぜか２回呼ばれることがあるので設けた
        if(!enterExit) 
        {
            ++GameManager.instance.level;
            enterExit = true;
        }
        yield return new WaitForSeconds(delayTime);
        //! シーンをリロードするのは効率が悪い
        GameManager.instance.ReloadScene();
    }

    public void LoseFood(int loss) 
    {
        _anim.SetTrigger("damaged");    

        food -= loss;

        var text = "-" + loss + "Food : " + food;

        CheckIsGameOver();
    }

    private void CheckIsGameOver() 
    {
        if(food <= 0) 
        {
            SoundManager.Instance.PlaySE(SE.Die);
            GameManager.instance.GameOver();
        }
    }

    #region Inputsystem Callbacks
    private void RecieveInputUp(InputAction.CallbackContext context) 
    {
        myUpdate(InputType.Up);
    }

    private void RecieveInputDown(InputAction.CallbackContext context)
    {
        myUpdate(InputType.Down);
    }
    private void RecieveInputLeft(InputAction.CallbackContext context)
    {
        if (!turnLeft) 
        {
            turnLeft = true;
            this.transform.localScale = new Vector3(-1, 1, 1); 
        }
        myUpdate(InputType.Left);
    }
    private void RecieveInputRight(InputAction.CallbackContext context)
    {
        myUpdate(InputType.Right);
        if(turnLeft) 
        {
            turnLeft = false;
            this.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    #endregion
}
