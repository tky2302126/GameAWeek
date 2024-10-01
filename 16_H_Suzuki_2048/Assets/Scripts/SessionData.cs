using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionData : MonoBehaviour
{
    public static SessionData instance{get; private set;}

    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public int score;
    public int BoardSize = 4;
}
