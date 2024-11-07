using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPresenter : MonoBehaviour
{
    private PlayerModel model;

    [SerializeField]
    private PlayerView view;

    public PlayerView View => view;

    // Start is called before the first frame update
    void Start()
    {
        model = new PlayerModel();
    }

    
}
