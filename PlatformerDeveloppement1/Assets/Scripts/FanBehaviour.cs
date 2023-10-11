using System;
using UnityEngine;

public class FanBehaviour : MonoBehaviour
{
    [SerializeField] private float fanForce = 0.5f;
    [SerializeField] private Vector3 fanDirection = Vector3.up;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding with the platform is the player.
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().AlterGravity(0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if the object colliding with the platform is the player.
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().transform.position += transform.up * fanForce * Time.deltaTime;
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