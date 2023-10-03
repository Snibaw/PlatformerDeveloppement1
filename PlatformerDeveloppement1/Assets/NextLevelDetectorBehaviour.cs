using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelDetectorBehaviour : MonoBehaviour
{
    private MainCameraMovement mainCam;
    [SerializeField] private Vector3 nextLevelCameraPosition;
    [SerializeField] private GameObject otherTP;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.GetComponent<MainCameraMovement>();
        otherTP.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            otherTP.SetActive(true);
            StartCoroutine(mainCam.MoveCamera(nextLevelCameraPosition));
        }
    }
}
