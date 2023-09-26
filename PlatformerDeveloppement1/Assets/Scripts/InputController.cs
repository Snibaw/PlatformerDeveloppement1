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
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementPlayer();
        

    }
    private void MovementPlayer()
    {
        //Horizontal Movement
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        playerMovement.Move(x);

        //Dash When Player press left button on controller
        if (Input.GetButtonDown("Fire3"))
        {
            playerMovement.Dash();
        }
        
    }
}
