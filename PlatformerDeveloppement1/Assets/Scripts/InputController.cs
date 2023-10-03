using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private float x,y;
    private PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        playerMovement.Move(x);

        //Sprint When Player press B on controller
        if (Input.GetButton("Fire2"))
        {
            playerMovement.Sprint();
        }
        else
        {
            playerMovement.StopSprint();
        }

        //Dash When Player press X on controller or Left Shift on keyboard 
        if (Input.GetButtonDown("Fire3"))
        {
            playerMovement.Dash();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            playerMovement.PressJumpButton();
        }
        else if(Input.GetButton("Fire1"))
        {
            playerMovement.HoldJumpButton();
        }
        // else if (Input.GetButtonUp("Jump"))
        // {
        //     playerMovement.StopPressJumpButton();
        //     // float jumpHeight = Mathf.Clamp01((Time.time - jumpTime) / maxJumpTime);
        // }
    }
}