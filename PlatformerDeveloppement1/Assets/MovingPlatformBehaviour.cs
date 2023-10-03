using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform currentTarget;
    public float speed = 0.1f;
    private int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        currentTarget = waypoints[currentIndex];
    }

    private void Update() {
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f) {
            currentIndex++;
            if (currentIndex >= waypoints.Length) {
                currentIndex = 0;
            }
            currentTarget = waypoints[currentIndex];
        }
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed);
    }
}
