using System;
using UnityEngine;

public class FanBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered" + collision.gameObject.name);
        // Check if the object colliding with the platform is the player.
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().AlterGravity(-1f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Triggered" + other.gameObject.name);
        // Check if the object colliding with the platform is the player.
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().ResetGravity();
        }
    }
}