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
    private bool hasCollideX = false;

    [Header("Dash")]
    //Dash
    [SerializeField] private float dashDistance = 10;
    [SerializeField] private float dashCooldown = 1f;
    private float dashCooldownTimer = 0;
    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 8;

    [Header("Jump")]
    [SerializeField] private float detectionCollisionOffsetY = 0.1f;
    private bool isGrounded;
    [SerializeField] private float maxJumpPower = 2;
    [SerializeField] private float decayJumpPower = 0.1f;
    private float jumpPower = 0;
    [SerializeField] private float maxJumpTime = 1;
    [SerializeField] private float gravity = 9.81f;
    private float speedY;
    private bool canDoubleJump = true;
    private bool isJumping;
    private float timeJumpButtonPressed;
    private bool hasCollideY = false;

    [Header("Environmental Speed")]
    [SerializeField] private float aerialHorizontalSpeedMultiplier = 0.7f;
    [SerializeField] private float slidingSpeedY = 0.3f;
    private bool isSliding = false;

    [Header("Wall Jump")]
    [SerializeField] 
    private float wallJumpHorizontalForceMax = 5;
    [SerializeField] 
    private float wallJumpHorizontalForceDecay = 0.1f;
    private float wallJumpHorizontalForce;
    private bool addHorizontalForceToJump = false;
    private bool wallIsOnTheRight = false;
    [SerializeField] 
    private float timeBetweenWallJumpAndMovement = 0.2f;
    private float timeSinceLastWallJump = 0;
    

    RaycastHit2D hitReturn;
    // [SerializeField] private float testvalue = 1;
    


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
        timeSinceLastWallJump += Time.deltaTime;

        GroundManagement();

        GravityManagement();
    }
    private void GravityManagement()
    {
        //Apply gravity if the player is not grounded
        if (!isGrounded)
        {
            if(!isCollidingWithObstacle || speedY >= 0) // If the player is sliding on a wall, the speed must be constant and not decrease
            {
                speedY -= gravity * Time.deltaTime;
                isSliding = false;
            }
            else
            {
                isSliding = true;
            }
        }   
        else
        {
            speedY = Mathf.Max(0, speedY);
            isSliding = false;
        }
            

        float environmentalSpeedY = !isGrounded && isCollidingWithObstacle && speedY < 0 ? slidingSpeedY : speedY;  // If the player is sliding on a wall, apply a sliding speed multiplier
        transform.Translate(new Vector3(0, environmentalSpeedY, 0));
    }
    private void GroundManagement()
    {
        float raycastLength = boxCollider.size.y/4 + Mathf.Abs(speedY) * Time.deltaTime;
        if (DetectCollision(transform.position - new Vector3(0, boxCollider.bounds.extents.y), Vector2.down, raycastLength, "Obstacle")) //Raycast to detect ground
        {
            canDoubleJump = true;
            isGrounded = true;
            if(hasCollideY == false) // Tp the player to the ground if it's his future position (only once)
            {
                hasCollideY = true;
                transform.position = new Vector3(transform.position.x, hitReturn.point.y + detectionCollisionOffsetY +boxCollider.size.y / 4 , transform.position.z);
            }
        }
        else
        {
            hasCollideY = false;
            isGrounded = false;
        }
    }
    public void Dash()
    {
        if(dashCooldownTimer <= 0)
        {
            dashCooldownTimer = dashCooldown;
            //Check if there is an obstacle in this direction
            if(DetectCollision(transform.position, new Vector3(lastX, 0, 0), dashDistance + boxCollider.size.x/2, "Obstacle"))
                transform.position += new Vector3(Mathf.Sign(lastX)*(hitReturn.distance - detectionCollisionOffsetX - boxCollider.size.x/2), 0, 0);
            else // If there is not, dash
                transform.position += new Vector3(dashDistance*Mathf.Sign(lastX), 0, 0);
        }
    }
    public void Move(float x)
    {  
        if(timeSinceLastWallJump < timeBetweenWallJumpAndMovement) return;

        Vector3 raycastDirection = new Vector3( x == 0 ? lastX : x, 0, 0);
        float environmentalSpeedX = isGrounded ? speed : speed * aerialHorizontalSpeedMultiplier;
        float raycastLength = boxCollider.size.x/2 + Mathf.Abs(x) * environmentalSpeedX * Time.deltaTime;
        
        //Check if there is an obstacle in this direction
        if(DetectCollision(transform.position, raycastDirection, raycastLength, "Obstacle"))
        {
            if(raycastDirection.x > 0) wallIsOnTheRight = true;
            else if(raycastDirection.x < 0) wallIsOnTheRight = false;
            PlayerCollideWhileHorizontalMovement(x);
        }
        else
        {
            isCollidingWithObstacle = false;
            hasCollideX = false;
        }
        
        // Move the player when there is no obstacle and an input
        if(!isCollidingWithObstacle)
        {
            transform.Translate(new Vector3(x, 0, 0) * environmentalSpeedX * Time.deltaTime);
        }

        // This is for inertia
        ManageInertia(x);
    }
    private void PlayerCollideWhileHorizontalMovement(float x)
    {
        isCollidingWithObstacle = true;
        if(hasCollideX == false) // Tp the player to the obstacle if it's his future position (only once)
        {
            hasCollideX = true;
            transform.position += new Vector3(Mathf.Sign(x)*(hitReturn.distance - detectionCollisionOffsetX -boxCollider.size.x/2), 0, 0);
        }
    }
    private void ManageInertia(float x)
    {
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
    private bool DetectCollision(Vector3 startPosition, Vector3 raycastDirection, float raycastLength, string targetTag)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPosition, raycastDirection, raycastLength); 

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.tag == targetTag) 
            {
                hitReturn = hit;
                return true;
            }
        }
        return false;
    }
    public void Sprint()
    {
        currentMaxSpeed = sprintSpeed;
    }
    public void StopSprint() // decays the sprint speed
    {
        currentMaxSpeed = Mathf.Max(maxSpeed, currentMaxSpeed -= Time.deltaTime * inertiaValue);
    }
    public void PressJumpButton()
    {
        wallJumpHorizontalForce = 0;
        if(isSliding)
        {
            wallJumpHorizontalForce = wallJumpHorizontalForceMax;
            canDoubleJump = true;
            timeSinceLastWallJump = 0;
        }
        else if(!isGrounded) 
        {
            if(!canDoubleJump)
            {
                //Avoid hold jump button condition
                timeJumpButtonPressed = maxJumpTime;
                return;
            }
            else
            {
                canDoubleJump = false;
            }
        }
        //Start jump
        timeJumpButtonPressed = Time.deltaTime;
        jumpPower = maxJumpPower;
        Jump();
    }
    public void HoldJumpButton()
    {
        timeJumpButtonPressed += Time.deltaTime;
        if(timeJumpButtonPressed > maxJumpTime) return;

        //Continue to add power to the jump, decay the power over time
        jumpPower = Mathf.Max(0, jumpPower - decayJumpPower * Time.deltaTime);
        wallJumpHorizontalForce = Mathf.Max(0, wallJumpHorizontalForce - wallJumpHorizontalForceDecay * Time.deltaTime);
        Jump();
    }

    private void Jump()
    {
        speedY = jumpPower;
        
        if(wallJumpHorizontalForce == 0) return;
        float horizontalMovement = wallIsOnTheRight ? -wallJumpHorizontalForce : wallJumpHorizontalForce; // The force depends on the direction of the player before jumping
        transform.Translate(new Vector3(horizontalMovement, 0, 0) * Time.deltaTime);
    }
}
