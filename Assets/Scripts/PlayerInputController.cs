using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputController : MonoBehaviour
{
    

    private Player player;
    
    private PlayerControls controls;
    // Start is called before the first frame update

    private bool firing = false;
    public float movY = 0;
    public float movX = 0;

    void Awake()
    {
        player = GetComponent<Player>();
        
        controls = new PlayerControls();
        //controls.GamePlay.Accelerate.performed += ctx => player.in
        controls.GamePlay.Accelerate.canceled += ctx => player.StopAcceleration();
        controls.GamePlay.Accelerate.started += ctx => player.StartAcceleration();
        
        /*
        controls.GamePlay.MoveUp.performed += ctx =>
        {
            if (mc) mc.PassiveMode = true;
            movY = 1;
        };
        controls.GamePlay.MoveDown.performed += ctx =>
        {
            if (mc) mc.PassiveMode = true;
            movY =  - 1;
        };
        controls.GamePlay.MoveLeft.performed += ctx =>
        {
            if (mc) mc.PassiveMode = true;
            movX =  - 1;
        };
        controls.GamePlay.MoveRight.performed += ctx =>
        {
            if (mc) mc.PassiveMode = true;
            movX = 1;
        };
        controls.GamePlay.MoveRight.canceled += ctx => movX = 0;
        controls.GamePlay.MoveLeft.canceled += ctx => movX = 0;
        controls.GamePlay.MoveUp.canceled += ctx => movY = 0;
        controls.GamePlay.MoveDown.canceled += ctx => movY = 0;

        controls.GamePlay.Move.performed += ctx =>
        {
            if (mc) mc.PassiveMode = true;
            var v = ctx.ReadValue<Vector2>();
            movX = v.x;
            movY = v.y;
        };
        controls.GamePlay.Move.canceled += ctx => { movX = 0; movY = 0; };
        /*
        controls.GamePlay..performed += ctx =>
        {
            ctx.ReadValue<Vector2>();
        };
        */
    }

    private void OnEnable()
    {
        controls?.GamePlay.Enable();// ? -> was editor errors on pause
    }

    private void OnDisable()
    {
        controls?.GamePlay.Disable(); // ? -> was editor errors on pause
    }
}
