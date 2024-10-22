using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーに追従するカメラを作成
/// キー入力で全体マップを表示する
/// </summary>
public class CameraMove : MonoBehaviour
{
    public InputActionAsset _input;
    public GameObject _target;
    private bool chaseTarget;
    private Camera _camera;
    private void Start()
    {
        if(_target != null) 
        {
            chaseTarget = true;
            transform.position = _target.transform.position;
        }
        _camera = GetComponent<Camera>();
        _input.FindAction("ChangeCameraMode").performed += OnCameraModeChange;
    }

    private void OnDestroy()
    {
        _input.FindAction("ChangeCameraMode").performed -= OnCameraModeChange;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(chaseTarget) 
        {
            var cameraPos = _target.transform.position;
            cameraPos.z = -10;
            transform.position = cameraPos;
        }
    }

    public void OnCameraModeChange(InputAction.CallbackContext context) 
    {
        chaseTarget = !chaseTarget;
        
        if(!chaseTarget ) 
        {
            Vector2Int boardSize = GameManager.instance.GetMapSize();
            transform.position = new Vector3((boardSize.x+2) / 2, (boardSize.y+2) / 2, -10);
            _camera.orthographicSize = 12; 
        }
        else 
        {
            _camera.orthographicSize = 5;
        }
    }


}
