using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxSpeed = 5;
    public void Move(float x)
    {
        transform.position += new Vector3(x, 0, 0) * maxSpeed * Time.deltaTime;
    }
}
