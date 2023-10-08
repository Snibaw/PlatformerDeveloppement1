using System;
using UnityEngine;

public class FanBehaviour : MonoBehaviour
{
    [SerializeField] private float fanForce = 0.5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object colliding with the platform is the player.
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().AlterGravity(-fanForce);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object colliding with the platform is the player.
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().ResetGravity();
        }
    }
}