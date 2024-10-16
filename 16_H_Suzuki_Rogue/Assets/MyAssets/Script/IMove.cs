using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//インターフェースを実装してインターフェースを抽象クラスに継承する
public interface IMove
{
    float moveTime {get; set;}

    bool Move(int xDir, int yDir, out RaycastHit2D hit);

    IEnumerator SmoothMove(Vector3 end);
    
    void AttemptMove<T>(int xDir, int yDir) where T : Component;

    void OnCantMove<T>(T component) where T : Component;

}
