using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject _gameManagerPrefab;
    private void Awake()
    {
        if(GameManager.instance == null) 
        {
            Instantiate(_gameManagerPrefab);
        }
    }
}
