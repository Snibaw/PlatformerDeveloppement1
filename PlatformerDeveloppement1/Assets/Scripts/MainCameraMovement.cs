using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private float speedFromOneLevelToAnother = 0.1f;
    public IEnumerator MoveCamera(Vector3 target)
    {
        targetPosition = target;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speedFromOneLevelToAnother);
            yield return null;
        }
    }
}
