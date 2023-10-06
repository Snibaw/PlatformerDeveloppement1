using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private float x,y;
    private PlayerMovement playerMovement;
    private CanvasManager canvasManager;
    private bool Fire2KeepPressed, Fire3Pressed, Fire1Pressed, Fire1KeepPressed;
    private TimerManager timerManager;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();
        playerMovement = GetComponent<PlayerMovement>();
        timerManager = GameObject.Find("TimerManager").GetComponent<TimerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        Fire1KeepPressed = false;
        Fire2KeepPressed = false;
        Fire3Pressed = false;
        Fire1Pressed = false;

        //Sprint When Player press B on controller
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
        

        //Pause (not physics related)
        //If player press Start on controller or Escape on keyboard
        if (Input.GetButtonDown("Cancel"))
        {
            canvasManager.PlayerPressPauseButton();
        }
        if(Input.GetButtonDown("ResetTimer"))
        {
            timerManager.ResetTimer();
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
    }
}