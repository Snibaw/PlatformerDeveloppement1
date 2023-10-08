using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed = 1.5f;

    private void Start()
    {
        GetComponent<Animator>().SetBool("isGoingRight", direction.x > 0);
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public float GetSpeed()
    {
        return speed;
    }
}