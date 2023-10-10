using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform currentTarget;
    private Vector3 direction;
    [SerializeField] private float speed = 1.5f;
    private int currentIndex = 0;
    private BoxCollider2D boxCollider2D;
    void Start()
    {
        currentTarget = waypoints[currentIndex];
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update() {
        
        // if(DetectCollisionWithPlayer()) return;

        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f) 
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length) 
            {
                currentIndex = 0;
            }
            currentTarget = waypoints[currentIndex];
        }
        direction = (currentTarget.position - transform.position).normalized * speed;
        transform.position += direction * Time.deltaTime;
    }
    public Vector3 GetDirection()
    {
        return direction;
    }
    // private bool DetectCollisionWithPlayer()
    // {
    //     Vector3 RaycastPosition;
    //     for(int i=0; i<7; i++)
    //     {
    //         RaycastPosition = transform.position + new Vector3(-boxCollider2D.size.x + i*boxCollider2D.size.x/3, 0, 0);
    //         RaycastHit2D hit = Physics2D.Raycast(RaycastPosition, Vector2.down, boxCollider2D.size.y/2, LayerMask.GetMask("Player"));
    //         Debug.DrawRay(RaycastPosition, Vector2.down * (boxCollider2D.size.y/2), Color.red);
    //         if(hit.collider != null)
    //         {
    //             Debug.Log(hit.collider.gameObject.name);
    //             return true;
    //         }
    //     }
    //     return false;
    // }
}
