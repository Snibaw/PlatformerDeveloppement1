using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public AnimationCurve curve;
    public IEnumerator Shaking(float duration)
    {
        if(!ToggleMenu.instance.isDashEffectEnabled)
            yield break;
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float strength = curve.Evaluate(elapsed / duration);
            transform.position = originalPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = originalPos;
    }
}
