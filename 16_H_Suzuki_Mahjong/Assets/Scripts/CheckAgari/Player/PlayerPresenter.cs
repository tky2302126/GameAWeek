using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPresenter : MonoBehaviour
{
    private PlayerModel model;

    [SerializeField]
    private PlayerView view;

    public PlayerView View => view;

    [SerializeField]
    private InputActionAsset input;

    void Start()
    {
        model = new PlayerModel();
        input.FindAction("Click").canceled += OnClick;
        input.Enable();
    }

    private void OnDisable()
    {
        input.FindAction("Click").canceled -= OnClick;
        input.Disable();
    }

    public void OnClick(InputAction.CallbackContext context) 
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.tag == "Tile")
        {
            var clickedTile = hit.collider.gameObject;

            Debug.Log("Tiles Clicked!");
        }

        if(hit.collider != null && hit.collider.tag == "Player") 
        {
            Debug.Log("Hand Clicked!");
        }
    }

    
}
