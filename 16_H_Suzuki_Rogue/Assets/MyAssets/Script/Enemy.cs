using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : absMove
{
    public int playerDamage;

    private Animator _anim;
    private Transform _target;
    private bool skipMove;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);

        _anim = GetComponent<Animator>();

        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        base.Start();
    }

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove) 
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy() 
    {
        int xDir = 0;
        int yDir = 0;

        if(Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon) 
        {
            yDir = _target.position.y > transform.position.y? 1 : -1 ;
        }
        else 
        {
            xDir = _target.position.x > transform.position.x ? 1 : - 1 ;
        }

        AttemptMove<Player>(xDir, yDir);

    }

    public override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.LoseFood(playerDamage);

        _anim.SetTrigger("attack");
        
    }
}
