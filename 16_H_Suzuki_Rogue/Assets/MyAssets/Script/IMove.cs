using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove 
{
    public const float moveTime = 0.1f;
    public LayerMask blockingLayer {  get; set; }

    public BoxCollider2D collider { get;  set; }
    public Rigidbody2D rigidbody { get; set; }
    public float inverseMoveTime { get; set; }
    public bool isMoving { get; set; }

    public void Init() 
    {
        collider = collider.GetComponent<BoxCollider2D>();
    }

    public void Start() 
    {
        
    }

}
