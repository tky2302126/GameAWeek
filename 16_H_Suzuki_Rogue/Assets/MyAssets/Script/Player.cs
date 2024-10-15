using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public enum InputType
{
    Up, Down, Left, Right
}

public class Player : absMove
{

    public float restartLevelDelay = 1f;
    public int pointPerFood = 10;
    public int wallDamage = 1;
    public TextMeshProUGUI _foodText;

    [SerializeField]
    private InputActionAsset _input;
    private Animator _anim;
    private int food;


    readonly int[] rowAxisMove =  { -1, 1, 0, 0 };

    readonly int[] columnAxisMove = {0,0,-1,1 };

    protected override void Start()
    {
       _anim = GetComponent<Animator>();

        //scriptableObjectに置き換えできそう
        food = GameManager.instance.playerFoodPoints;

        var text = "Food : " + food;

        _foodText.SetText(text);

        _input.FindAction("Up").performed += RecieveInputUp;
        _input.FindAction("Down").performed += RecieveInputDown;
        _input.FindAction("Left").performed += RecieveInputLeft;
        _input.FindAction("Right").performed += RecieveInputRight;

        base.Start();
    }

    

    private void OnDisable()
    {
        //次のフロアへフードの値を引き継ぐ
        GameManager.instance.playerFoodPoints = food;
    }

   //myUpdateにして入力を受けたら呼び出すようにする
   //2048から持ってくるとよさそう
    void myUpdate(InputType input)
    {
        if (!GameManager.instance.playersTurn) { return; }

        AttemptMove<Wall>(rowAxisMove[(int)input], columnAxisMove[(int)input]);


    }

   public override void AttemptMove<T>(int xDir,int yDir) 
    {
        food--;

        var text = "Food : " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIsGameOver();

        GameManager.instance.playersTurn =false;
    }

    public override void OnCantMove<T>(T component) 
    {
        Wall hitWall = component as Wall;

        hitWall.DamageWall(wallDamage);

        _anim.SetTrigger("attack");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit") 
        {
            Invoke("Restart",restartLevelDelay);

            enabled = false;
        }

        else if(other.tag == "Food") 
        {
            food += pointPerFood;

            var text = "+" + pointPerFood + "Food : " + food;

            other.gameObject.SetActive(false);
        }
        
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
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
            GameManager.instance.GameOver();
        }
    }

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
        myUpdate(InputType.Left);
    }
    private void RecieveInputRight(InputAction.CallbackContext context)
    {
        myUpdate(InputType.Right);
    }

}
