using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTimer : MonoBehaviour
{
    private TimerManager timerManager;
    // Start is called before the first frame update
    void Start()
    {
        timerManager = GameObject.Find("TimerManager").GetComponent<TimerManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            timerManager.EndTimer(true);
        }
    }
}