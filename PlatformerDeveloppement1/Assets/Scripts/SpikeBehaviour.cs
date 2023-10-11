using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            //EndGame();
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if(playerMovement.isDead) return;
            
            playerMovement.Die();
            Invoke("ShowEndGameMenu", 1f);
        }
    }
    private void ShowEndGameMenu()
    {
        GameObject.Find("Canvas").GetComponent<CanvasManager>().OpenPauseMenu(_isGameFinished: true);
    }
}
