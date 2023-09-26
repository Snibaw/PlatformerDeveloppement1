using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("HorizontalMovement")]
    //HorizontalMovement
    [SerializeField] private float maxSpeed = 5;
    [SerializeField] private float detectionCollisionOffset = -0.1f;
    [SerializeField] private float inertiaValue = 8;
    private float speed;
    private float lastX;
    private BoxCollider2D boxCollider;
    private bool isCollidingWithObstacle = false;

    [Header("Dash")]
    //Dash
    [SerializeField] private float dashDistance = 10;
    [SerializeField] private float dashCooldown = 1f;
    private float dashCooldownTimer = 0;



    private void Start() 
    {
        speed = 0;
        boxCollider = GetComponent<BoxCollider2D>();
        dashCooldownTimer = 0;
    }
    private void Update() 
    {
        dashCooldownTimer -= Time.deltaTime;
    }
    public void Dash()
    {
        
        if(dashCooldownTimer <= 0)
        {
            Debug.Log("Dash");
            dashCooldownTimer = dashCooldown;

            //Check if there is an obstacle in this direction
            Vector3 raycastDirection = new Vector3(lastX, 0, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, dashDistance + detectionCollisionOffset + boxCollider.size.x/2);

            if(hit.collider != null)
            {
                if(hit.collider.gameObject.tag == "Obstacle")
                {
                    transform.position += new Vector3(Mathf.Sign(lastX)*(hit.distance - detectionCollisionOffset - boxCollider.size.x/2), 0, 0);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, detectionCollisionOffset + boxCollider.size.x/2);

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
            speed = Mathf.Min(maxSpeed, speed + Time.deltaTime * inertiaValue);
            lastX = x;
        }
    }
}
