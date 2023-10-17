using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Basic Horizontal Movement")]
    //HorizontalMovement
    [SerializeField]
    private float maxSpeed = 5;

    private float replacementPositionXOffset = 0;
    private float raycastPositionXOffset = 0.05f;
    [SerializeField] private float inertiaValue = 8;
    private float currentMaxSpeed;
    private float speed;
    private float lastX;
    private float x;
    private BoxCollider2D boxCollider;
    private bool isCollidingWithObstacle = false;
    private bool isCollidingWithAllRayOnObstacle = false;
    private bool hasCollideX = false;

    [SerializeField] private float sprintSpeed = 6;

    [Header("Dash")]
    //Dash
    [SerializeField]
    private float dashDistance = 10;

    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    [SerializeField] private float dashDuration = 0.1f;
    private float currentDashDuration = 0;
    private float dashCooldownTimer = 0;
    

    [Header("Jump")] 
    [SerializeField] private float maxJumpPower = 2;
    private float replacementPositionYOffset = 0.3f;
    private float raycastPositionYOffset = 0f;
    private bool isGrounded;
    private bool wasGrounded;
    [SerializeField] private float decayJumpPower = 0.1f;
    private float jumpPower = 0;
    [SerializeField] private float maxJumpTime = 1;
    [SerializeField] private float gravity = 9.81f;
    private float gravityMultiplier = 1;
    private bool isGravityOn = true;
    private float speedY;
    private bool canDoubleJump = true;
    private bool isJumping;
    private float timeJumpButtonPressed;
    private bool hasCollideY = false;
    private bool collideWithTop = false;

    [Header("Environmental Speed")] [SerializeField]
    private float aerialHorizontalSpeedMultiplier = 0.7f;
    [SerializeField] private float slidingSpeedY = 0.3f;
    [HideInInspector] public bool isSliding = false;
    private float environmentalSpeedX = 0;
    private float environmentalSpeedY = 0;
    private Vector3 platformDirection;

    [Header("Wall Jump")] [SerializeField] private float wallJumpHorizontalForceMax = 5;
    [SerializeField] private float wallJumpHorizontalForceDecay = 0.1f;
    private float wallJumpHorizontalForce;
    private bool addHorizontalForceToJump = false;
    private bool wallIsOnTheRight = false;
    [SerializeField] private float timeBetweenWallJumpAndMovement = 0.2f;
    private float timeSinceLastWallJump = 0;

    [Header("CoyotteTime & bufferJumpTime")] [SerializeField]
    private float coyotteTime = 0.2f;

    [SerializeField] private float bufferJumpTime = 0.2f;
    private float coyotteTimeTimer = 0;
    private float bufferJumpTimeTimer = 0;
    RaycastHit2D hitReturn;

    


    private void Start()
    {
        speed = 0;
        dashCooldownTimer = 0;
        timeJumpButtonPressed = 0;
        isGrounded = false;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        dashCooldownTimer -= Time.deltaTime;
        timeSinceLastWallJump += Time.deltaTime;
        coyotteTimeTimer -= Time.deltaTime;
        bufferJumpTimeTimer -= Time.deltaTime;

        GroundManagement();
        if(speedY >=0)
            FloorManagement();

        WallManagement();

        GravityManagement();

        ApplyDash();
    }

    private void GravityManagement()
    {
        //Apply gravity if the player is not grounded
        if (!isGrounded)
        {

            if (isCollidingWithAllRayOnObstacle && (speedY < 0 ||environmentalSpeedY < 0)) // If the player is sliding on a wall, the speed must be constant and not decrease
            {
                isSliding = true;
            }
            else
            {
                speedY -= gravity * gravityMultiplier * Time.deltaTime;
                isSliding = false;
            }
        }
        else
        {
            speedY = Mathf.Max(0, speedY);
            isSliding = false;
        }


        environmentalSpeedY =
            !isGrounded && isCollidingWithAllRayOnObstacle && speedY < 0
                ? slidingSpeedY
                : speedY; // If the player is sliding on a wall, apply a sliding speed multiplier
        transform.Translate(new Vector3(0, environmentalSpeedY, 0));
    }
    

    private void GroundManagement()
    {
        wasGrounded = isGrounded;
        float raycastLength = 2*boxCollider.size.y / 8 + Mathf.Abs(speedY) * Time.deltaTime;
        bool hasCollided = false;
        Vector3 raycastPosition;
        for(int i=0; i<7; i++)
        {
            raycastPosition = transform.position + new Vector3(-3*boxCollider.size.x/8 + i*boxCollider.size.x/8, -boxCollider.bounds.extents.y - raycastPositionYOffset+ boxCollider.size.y/8);
            Debug.DrawRay(raycastPosition, new Vector3(0, -1, 0) * raycastLength, Color.red);
            if(DetectCollision(raycastPosition, new Vector3(0, -1, 0), raycastLength, "Obstacle"))
            {
                hasCollided = true;
            }
        }

        if (hasCollided)
        {
            //For moving platform
            if (hitReturn.collider.gameObject.name == "MovingPlatform")
            {
                platformDirection =
                    hitReturn.collider.gameObject.GetComponent<MovingPlatformBehaviour>().GetDirection();
                transform.position += platformDirection * Time.deltaTime;
            }
            //For conveyor belt platform
            if (hitReturn.collider.gameObject.name == "ConveyorBelt")
            {
                platformDirection =
                    hitReturn.collider.gameObject.GetComponent<ConveyorBeltBehaviour>().GetDirection();
                float platformSpeed =
                    hitReturn.collider.gameObject.GetComponent<ConveyorBeltBehaviour>().GetSpeed();
                transform.position += platformDirection * platformSpeed * Time.deltaTime;
            }

            //Start coyotteTime and Check for bufferJumpTime
            if (bufferJumpTimeTimer > 0)
            {
                PressJumpButton();
            }

            coyotteTimeTimer = coyotteTime;

            canDoubleJump = true;
            isGrounded = true;
            if (hasCollideY == false) // Tp the player to the ground if it's his future position (only once)
            {
                hasCollideY = true;
                if(hitReturn.transform.gameObject.GetComponent<BoxCollider2D>() != null)
                    transform.position = new Vector3(transform.position.x, hitReturn.transform.gameObject.GetComponent<BoxCollider2D>().bounds.max.y + boxCollider.size.y / 2, transform.position.z);
                else
                    transform.position = new Vector3(transform.position.x, hitReturn.point.y + replacementPositionYOffset + boxCollider.size.y / 8, transform.position.z);
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
        Vector3 raycastPosition1 =
            transform.position + new Vector3(2*boxCollider.bounds.extents.x/3, boxCollider.bounds.extents.y);
        Vector3 raycastPosition2 =
            transform.position + new Vector3(-2*boxCollider.bounds.extents.x/3, boxCollider.bounds.extents.y);
        
        float raycastLength = Mathf.Abs(speedY) * Time.deltaTime;
        Vector3 raycastPosition;
        bool hasCollided = false;
        for(int i=0; i<7; i++)
        {
            raycastPosition = transform.position + new Vector3(-3*boxCollider.size.x/8 + i*boxCollider.size.x/8, boxCollider.bounds.extents.y);
            Debug.DrawRay(raycastPosition, new Vector3(lastX, 0, 0) * raycastLength, Color.blue);
            if(DetectCollision(raycastPosition, new Vector3(lastX, 0, 0), raycastLength, "Obstacle"))
            {
                hasCollided = true;
            }
        }
        if (hasCollided)
        {
            timeJumpButtonPressed = 2 * maxJumpTime; // Stop the jump if the player is colliding with the ceiling
            if(collideWithTop == false)
            {
                transform.position = new Vector3(transform.position.x, hitReturn.point.y - boxCollider.size.y / 2, transform.position.z);
            }
            collideWithTop = true;
            speedY = -speedY / 2;
        }
        else
        {
            collideWithTop = false;
        }
    }

    public void Dash()
    {
        if (dashCooldownTimer <= 0)
        {
            dashCooldownTimer = dashCooldown;
            isDashing = true;
            timeJumpButtonPressed = maxJumpTime * 2; //Avoid hold jump button condition
            speedY = 0;
        }
    }

    public void ApplyDash()
    {
        if (isDashing)
        {
            if (currentDashDuration < dashDuration)
            {
                bool hasCollided = false;
                Vector3 raycastPosition;
                for(int i=0; i<7; i++)
                {
                    raycastPosition = transform.position + new Vector3(Mathf.Sign(lastX) * (boxCollider.bounds.extents.x + raycastPositionXOffset), -3*boxCollider.size.y / 8 + i*boxCollider.size.y/8);
                    Debug.DrawRay(raycastPosition, new Vector3(lastX, 0, 0) * dashDistance, Color.yellow);
                    if(DetectCollision(raycastPosition, new Vector3(lastX, 0, 0), dashDistance, "Obstacle"))
                    {
                        hasCollided = true;
                    }
                }

                //Check if there is an obstacle in this direction
                if (hasCollided)
                {
                    transform.position += new Vector3(Mathf.Sign(lastX) * (hitReturn.distance - replacementPositionXOffset - boxCollider.size.x / 2), 0, 0);
                    isDashing = false;
                }
                else
                {
                    transform.position += new Vector3(dashDistance / dashDuration * Time.deltaTime * Mathf.Sign(lastX),0, 0);
                    currentDashDuration += Time.deltaTime;
                }
            }
            else
            {
                isDashing = false;
                isGravityOn = true;
                currentDashDuration = 0;
            }
        }
    }
    private void WallManagement()
    {
        Vector3 raycastDirection = new Vector3(x == 0 ? lastX : x, 0, 0);
        environmentalSpeedX = isGrounded ? speed : speed * aerialHorizontalSpeedMultiplier;
        float raycastLength = boxCollider.size.x/8 + Mathf.Abs(x) * environmentalSpeedX * Time.deltaTime;
        
        List<Vector3> raycastPositions = new List<Vector3>();

        bool collidedWithRay2 = true;
        bool collidedWithRay1 = false;
        for(int i =0; i<7; i++)
        {
            raycastPositions.Add(transform.position + new Vector3(Mathf.Sign(raycastDirection.x) * (boxCollider.bounds.extents.x + raycastPositionXOffset), -3*boxCollider.size.y / 8 + i*boxCollider.size.y/8));
            Debug.DrawRay(raycastPositions[i], raycastDirection * raycastLength, Color.green);
            if(DetectCollision(raycastPositions[i], raycastDirection, raycastLength, "Obstacle"))
            {
                collidedWithRay1 = true;
            }
            else
            {
                collidedWithRay2 = false;
            }
        }
        //Check if there is an obstacle in this direction
        if (collidedWithRay1)
        {
            if (raycastDirection.x > 0) wallIsOnTheRight = true;
            else if (raycastDirection.x < 0) wallIsOnTheRight = false;
                    isCollidingWithObstacle = true;

            if (hasCollideX == false) // Tp the player to the obstacle if it's his future position (only once)
            {
                hasCollideX = true;
                transform.position = new Vector3(
                    hitReturn.point.x - replacementPositionXOffset - Mathf.Sign(raycastDirection.x) * boxCollider.size.x / 2,
                    transform.position.y, transform.position.z);
            }

            if(collidedWithRay2)
            {
                isCollidingWithAllRayOnObstacle = true;
            }

            else
            {
                isCollidingWithAllRayOnObstacle = false;
            }
        }
        else
        {
            isCollidingWithAllRayOnObstacle = false;
            isCollidingWithObstacle = false;
            hasCollideX = false;
        }

    }

    public void Move(float _x)
    {
        if (timeSinceLastWallJump < timeBetweenWallJumpAndMovement) return;

        x = _x;

        // Move the player when there is no obstacle and an input
        if (!isCollidingWithObstacle)
        {
            transform.Translate(new Vector3(x, 0, 0) * environmentalSpeedX * Time.deltaTime);
        }
        // This is for inertia
        ManageInertia();
    }


    private void ManageInertia()
    {
        if (x == 0)
        {
            speed = Mathf.Max(0, speed - Time.deltaTime * inertiaValue);
            if (!isCollidingWithObstacle) transform.Translate(new Vector3(lastX, 0, 0) * speed * Time.deltaTime);
        }
        else
        {
            speed = Mathf.Min(currentMaxSpeed, speed + Time.deltaTime * inertiaValue);
            lastX = x;
        }
    }

    private bool DetectCollision(Vector3 startPosition, Vector3 raycastDirection, float raycastLength, string targetTag)
    {
        //Only detect Default layer
        RaycastHit2D hit = Physics2D.Raycast(startPosition, raycastDirection, raycastLength, LayerMask.GetMask("Default"));

        if (hit.collider != null)
        {

            if (hit.collider.gameObject.tag == targetTag)
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
        if (isSliding)
        {
            wallJumpHorizontalForce = wallJumpHorizontalForceMax;
            canDoubleJump = true;
            timeSinceLastWallJump = 0;
        }
        else if (!isGrounded && coyotteTimeTimer < 0)
        {
            if (!canDoubleJump)
            {
                //Avoid hold jump button condition
                bufferJumpTimeTimer = bufferJumpTime;
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
        if (timeJumpButtonPressed > maxJumpTime) return;

        //Continue to add power to the jump, decay the power over time
        jumpPower = Mathf.Max(0, jumpPower - decayJumpPower * Time.deltaTime);
        wallJumpHorizontalForce = Mathf.Max(0, wallJumpHorizontalForce - wallJumpHorizontalForceDecay * Time.deltaTime);
        Jump();
    }

    private void Jump()
    {
        speedY = jumpPower;
        if (wallJumpHorizontalForce == 0) return;
        float horizontalMovement =
            wallIsOnTheRight
                ? -wallJumpHorizontalForce
                : wallJumpHorizontalForce; // The force depends on the direction of the player before jumping
        transform.Translate(new Vector3(horizontalMovement, 0, 0) * Time.deltaTime);
    }
}
