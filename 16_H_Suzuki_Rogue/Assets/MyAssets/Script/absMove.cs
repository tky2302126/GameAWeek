using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class absMove : MonoBehaviour, IMove
{
    public float moveTime { get; set; } = 0.1f;

    public LayerMask blockingLayer { get; set; }

    private BoxCollider2D boxCollider;

    private Rigidbody2D rbody2D;

    private float inverseMoveTime;

    private bool isMoving;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        rbody2D = GetComponent<Rigidbody2D>();

        inverseMoveTime = 1f / moveTime;
    }

    //out をつけたら関数内で結果代入（義務）
    public bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        bool result = false;

        Vector2 start = transform.position;

        Vector2 end = start + new Vector2(xDir, yDir);

        //raycastが当たらないようにする
        boxCollider.enabled = false;

        //衝突の確認に
        hit = Physics2D.Linecast(start, end, blockingLayer);

        boxCollider.enabled = true;

        //hitなし
        if (hit.transform == null && !isMoving)
        {
            StartCoroutine(SmoothMove(end));
            result = true;
        }

        return result;
    }

    //スプライトを移動させる
    public IEnumerator SmoothMove(Vector3 end)
    {
        isMoving = true;
        float sqrRemainDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainDistance > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(rbody2D.position, end, inverseMoveTime * Time.deltaTime);
            rbody2D.MovePosition(newPos);

            sqrRemainDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        rbody2D.MovePosition(end);

        isMoving = false;
    }
    //overrideしたいので仮想関数にした
    public virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null) { return; }

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    //継承先で実装
    public virtual void OnCantMove<T>(T component) where T : Component
    {

    }

}