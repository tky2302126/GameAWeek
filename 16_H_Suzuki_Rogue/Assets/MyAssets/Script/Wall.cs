using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int Hp = 3;

    public void DamageWall(int loss) 
    {
        Hp -= loss;

        if(Hp <= 0) 
        {
            gameObject.SetActive(false);
        }
    }
}
