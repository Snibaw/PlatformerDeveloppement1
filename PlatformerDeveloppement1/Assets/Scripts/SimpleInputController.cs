using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInputController : MonoBehaviour
{
    private float x,y;
    private SimplePlayerMovement playerMovement;
    private CanvasManager canvasManager;
    private bool Fire2KeepPressed, Fire3Pressed, Fire1Pressed, Fire1KeepPressed;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        playerMovement = GetComponent<SimplePlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        //Sprint When Player press LB on controller
        if (Input.GetButton("Fire2"))
        {
            Fire2KeepPressed = true;
        }

        //Dash When Player press X on controller or Left Shift on keyboard 
        if (Input.GetButtonDown("Fire3"))
        {
            Fire3Pressed = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Fire1Pressed = true;
        }

        if(Input.GetButton("Fire1"))
        {
            Fire1KeepPressed = true;
        }
    }
    void FixedUpdate()
    {
        playerMovement.Move(x);

        if(Fire2KeepPressed)
        {
            playerMovement.Sprint();
        }
        else
        {
            playerMovement.StopSprint();
        }

        if(Fire3Pressed)
        {
            playerMovement.Dash();
        }

        if(Fire1Pressed)
        {
            playerMovement.PressJumpButton();
        }
        else if(Fire1KeepPressed)
        {
            playerMovement.HoldJumpButton();
        }

        Fire1KeepPressed = false;
        Fire2KeepPressed = false;
        Fire3Pressed = false;
        Fire1Pressed = false;
    }
}