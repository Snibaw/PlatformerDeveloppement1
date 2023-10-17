using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EchoEffect : MonoBehaviour
{
    public GameObject echoPrefab;
    [SerializeField] private Transform transformCopyModel;
    [SerializeField] private Color echoColor;
    private Vector3 startPos, endPos;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;

    private void Start() {
        Volume volume = GameObject.Find("Global Volume").GetComponent<Volume>();
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out lensDistortion);
    }

    public void SpawnEcho(Vector3 positionToSpawn, Color echoColorTemp)
    {
        Transform transformTemp = transformCopyModel == null ? transform : transformCopyModel;
        GameObject echo = Instantiate(echoPrefab, positionToSpawn, transformTemp.rotation);
        echo.GetComponent<SpriteRenderer>().color = echoColorTemp;
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
        if(!ToggleMenu.instance.isDashEffectEnabled)
            yield break;

        Color echoColorTemp = echoColor;
        for (int i = 0; i < numberOfEchoes; i++)
        {
            SpawnEcho(Vector3.Lerp(startPos, endPos, i / (float)numberOfEchoes), echoColorTemp);
            //Make echoColor Brighter
            echoColorTemp = new Color(echoColorTemp.r + 0.1f, echoColorTemp.g + 0.1f, echoColorTemp.b + 0.1f, echoColorTemp.a);
            yield return new WaitForSeconds(timeBtwSpawn);
        }
        chromaticAberration.intensity.value = 0f;
        lensDistortion.intensity.value = 0f;
    }
}
