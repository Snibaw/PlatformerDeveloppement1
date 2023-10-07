using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelDetectorBehaviour : MonoBehaviour
{
    private MainCameraMovement mainCam;
    [SerializeField] private Vector3 nextLevelCameraPosition;
    void Start()
    {
        mainCam = Camera.main.GetComponent<MainCameraMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(mainCam.MoveCamera(nextLevelCameraPosition));
        }
    }
}
