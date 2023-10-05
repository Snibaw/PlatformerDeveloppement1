using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoEffect : MonoBehaviour
{
    public GameObject echoPrefab;
    [SerializeField] private Transform transformCopyModel;
    private Vector3 startPos, endPos;

    public void SpawnEcho(Vector3 positionToSpawn)
    {
        Transform transformTemp = transformCopyModel == null ? transform : transformCopyModel;
        GameObject echo = Instantiate(echoPrefab, positionToSpawn, transformTemp.rotation);
        Destroy(echo, 1f);
    }
    public void SetStartPos(Vector3 startPos)
    {
        this.startPos = startPos;
    }
    public void SetEndPos(Vector3 endPos)
    {
        this.endPos = endPos;
    }
    public IEnumerator SpawnEveryEchoes(float timeBtwSpawn, int numberOfEchoes)
    {
        for (int i = 0; i < numberOfEchoes; i++)
        {
            SpawnEcho(Vector3.Lerp(startPos, endPos, i / (float)numberOfEchoes));
            yield return new WaitForSeconds(timeBtwSpawn);
        }
    }
}
