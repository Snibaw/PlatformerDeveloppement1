using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{


    [Header("Basic Horizontal Movement")]
    //HorizontalMovement
    [SerializeField] private float maxSpeed = 5;
    [SerializeField] private float detectionCollisionOffsetX = -0.1f;
    [SerializeField] private float inertiaValue = 8;
    private float currentMaxSpeed;
    private float speed;
    private float lastX;
    private BoxCollider2D boxCollider;
    private bool isCollidingWithObstacle = false;

    [Header("Dash")]
    //Dash
    [SerializeField] private float dashDistance = 10;
    [SerializeField] private float dashCooldown = 1f;
    private float dashCooldownTimer = 0;
    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 8;

    [Header("Jump")]
    private float speedX;
    private float speedY;
    [SerializeField] private float detectionCollisionOffsetY = 0.1f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float maxJumpPower = 2;
    [SerializeField] private float decayJumpPower = 0.1f;
    private float jumpPower = 0;
    [SerializeField] private float maxJumpTime = 1;
    [SerializeField] private float gravity = 9.81f;
    private bool isJumping;
    private float timeJumpButtonPressed;


    private void Start() 
    {
        speed = 0;
        boxCollider = GetComponent<BoxCollider2D>();
        dashCooldownTimer = 0;
        timeJumpButtonPressed = 0;
        isGrounded = false;
    }
    private void Update() 
    {
        dashCooldownTimer -= Time.deltaTime;

        CheckGrounded();
        if (!isGrounded)
            speedY -= gravity * Time.deltaTime;
        else
            speedY = Mathf.Max(0, speedY);
        transform.Translate(new Vector3(0, speedY, 0));
    }
    public void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), detectionCollisionOffsetY + boxCollider.size.y / 2);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    public void Dash()
    {
        
        if(dashCooldownTimer <= 0)
        {
            Debug.Log("Dash");
            dashCooldownTimer = dashCooldown;

            //Check if there is an obstacle in this direction
            Vector3 raycastDirection = new Vector3(lastX, 0, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, dashDistance + detectionCollisionOffsetX + boxCollider.size.x/2);

            if(hit.collider != null)
            {
                if(hit.collider.gameObject.tag == "Obstacle")
                {
                    transform.position += new Vector3(Mathf.Sign(lastX)*(hit.distance - detectionCollisionOffsetX - boxCollider.size.x/2), 0, 0);
                }
                else
                {
                    transform.position += new Vector3(dashDistance*Mathf.Sign(lastX), 0, 0);
                }
            }
            else
            {
                transform.position += new Vector3(dashDistance*Mathf.Sign(lastX), 0, 0);
            }
            

        }
    }
    public void Move(float x)
    {
        //Check if there is an obstacle in this direction     
        Vector3 raycastDirection = new Vector3( x == 0 ? lastX : x, 0, 0);
        //Draw Raycast depending on the direction of the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, detectionCollisionOffsetX + boxCollider.size.x/2);

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "Obstacle")
            {   
                isCollidingWithObstacle = true;
            }
            else
            {
                isCollidingWithObstacle = false;
            }
        }
        else
        {
            isCollidingWithObstacle = false;
        }
        
        // Move the player when there is no obstacle and an input
        if(!isCollidingWithObstacle)
        {
            transform.Translate(new Vector3(x, 0, 0) * speed * Time.deltaTime);
        }

        // This is for inertia
        if(x == 0)
        {
            speed = Mathf.Max(0, speed - Time.deltaTime * inertiaValue);
            if(!isCollidingWithObstacle) transform.Translate(new Vector3(lastX, 0, 0) * speed * Time.deltaTime);
        }
        else
        {
            speed = Mathf.Min(currentMaxSpeed, speed + Time.deltaTime * inertiaValue);
            lastX = x;
        }
    }
    public void Sprint()
    {
        currentMaxSpeed = sprintSpeed;
    }
    public void StopSprint()
    {
        currentMaxSpeed = Mathf.Max(maxSpeed, currentMaxSpeed -= Time.deltaTime * inertiaValue);
    }
    public void PressJumpButton()
    {
        if(!isGrounded) 
        {
            timeJumpButtonPressed = maxJumpTime+1;
            return;
        }
        timeJumpButtonPressed = Time.deltaTime;
        jumpPower = maxJumpPower;
        Jump();
    }
    public void HoldJumpButton()
    {
        timeJumpButtonPressed += Time.deltaTime;
        if(timeJumpButtonPressed > maxJumpTime) return;

        jumpPower = Mathf.Max(0, jumpPower - decayJumpPower * Time.deltaTime);
        Jump();
    }
    // public void StopPressJumpButton()
    // {
    //     float jumpHeightPercentage = Mathf.Clamp01((Time.time - timeJumpButtonPressed) / maxJumpTime);
    //     Jump(jumpHeightPercentage);
    // }
    private void Jump()
    {
        speedY = jumpPower;
    }
}
