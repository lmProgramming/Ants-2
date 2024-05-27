using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimation : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(StartAnimation());
    }

    public AnimationCurve popAnimationCurve;
    public IEnumerator StartAnimation()
    {
        float time = 0;

        Vector3 originalScale = transform.parent.localScale;

        while (popAnimationCurve[popAnimationCurve.length - 1].time > time)
        {
            float currentScaleMultiplier = popAnimationCurve.Evaluate(time);

            transform.parent.localScale = originalScale * currentScaleMultiplier;

            time += GameInput.unscaledDeltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
