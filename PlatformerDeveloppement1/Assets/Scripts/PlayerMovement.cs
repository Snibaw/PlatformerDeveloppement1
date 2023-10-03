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
    private bool collideWithTop = false;

    [Header("Environmental Speed")]
    [SerializeField] private float aerialHorizontalSpeedMultiplier = 0.7f;
    [SerializeField] private float slidingSpeedY = 0.3f;
    public bool isSliding = false;
    private Vector3 platformDirection;

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

    [Header("CoyotteTime & bufferJump")]
    [SerializeField] private float coyotteTime = 0.2f;
    [SerializeField] private float bufferJump = 0.2f;
    private float coyotteTimeTimer = 0;
    private float bufferJumpTimer = 0;
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
        coyotteTimeTimer -= Time.deltaTime;
        bufferJumpTimer -= Time.deltaTime;

        GroundManagement();
        FloorManagement();

        GravityManagement();
    }
    private void GravityManagement()
    {
        //Apply gravity if the player is not grounded
        if (!isGrounded)
        {
            if(isCollidingWithObstacle && speedY < 0 && !collideWithTop) // If the player is sliding on a wall, the speed must be constant and not decrease
            {
                isSliding = true;
            }
            else
            {
                speedY -= gravity * Time.deltaTime;
                isSliding = false;
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
        Vector3 raycastPosition1 = transform.position - new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
        Vector3 raycastPosition2 = transform.position - new Vector3(-boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
        if (DetectCollision(raycastPosition1, new Vector3(0, -1, 0), raycastLength, "Obstacle") || DetectCollision(raycastPosition2, new Vector3(0, -1, 0), raycastLength, "Obstacle"))
        {
            //For moving platform
            if(hitReturn.collider.gameObject.name == "MovingPlatform")
            {
                platformDirection = hitReturn.collider.gameObject.GetComponent<MovingPlatformBehaviour>().GetDirection();
                transform.position += platformDirection * Time.deltaTime;
            }

            //Start coyotteTime and Check for bufferJump
            if(bufferJumpTimer > 0)
            {
                PressJumpButton();
            }
            coyotteTimeTimer = coyotteTime;

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
    private void FloorManagement()
    {
        float raycastLength = Mathf.Abs(speedY) * Time.deltaTime;
        Vector3 raycastPosition1 = transform.position + new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
        Vector3 raycastPosition2 = transform.position + new Vector3(-boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
        if (DetectCollision(raycastPosition1, new Vector3(0, 1, 0), raycastLength, "Obstacle") || DetectCollision(raycastPosition2, new Vector3(0, 1, 0), raycastLength, "Obstacle"))
        {
            timeJumpButtonPressed = 2*maxJumpTime; // Stop the jump if the player is colliding with the ceiling
            collideWithTop = true;
            speedY = -speedY/2;
        }
        else
        {
            collideWithTop = false;
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
        Vector3 raycastPosition1 = transform.position - new Vector3(0, boxCollider.bounds.extents.y/2);
        Vector3 raycastPosition2 = transform.position + new Vector3(0, boxCollider.bounds.extents.y/2);
        
        //Check if there is an obstacle in this direction
        if(DetectCollision(raycastPosition1, raycastDirection, raycastLength, "Obstacle") || DetectCollision(raycastPosition2, raycastDirection, raycastLength, "Obstacle"))
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
        else if(!isGrounded && coyotteTimeTimer < 0) 
        {
            if(!canDoubleJump)
            {
                //Avoid hold jump button condition
                bufferJumpTimer = bufferJump;
                timeJumpButtonPressed = maxJumpTime;
                return;
            }
            else
            {
                canDoubleJump = false;
            }
        }
        //Start jump
        coyotteTimeTimer = -1;
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
