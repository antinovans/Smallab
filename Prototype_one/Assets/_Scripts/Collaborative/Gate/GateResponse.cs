using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateResponse : MonoBehaviour
{
    private bool isShaking;
    public AnimationCurve magnitudeCurve;
    public float duration;
    public float magnitude;
    private void Start()
    {
        EventManager.current.onGateColliderEnter += Shake;
        isShaking = false;
    }
    public void Shake()
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeGate());
        }
    }
    private IEnumerator ShakeGate()
    {
        isShaking = true;
        Vector3 originPos = transform.position;

        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            float curvePercent = magnitudeCurve.Evaluate(elapsed / duration);
            float x = curvePercent * magnitude * UnityEngine.Random.Range(-1f, 1f);
            float z = curvePercent * magnitude * UnityEngine.Random.Range(-1f, 1f);
            transform.position = new Vector3(originPos.x + x, originPos.y, originPos.z + z);

            elapsed += Time.deltaTime;

            yield return 0.1f;
        }
        isShaking = false;
        transform.position = originPos;
    }
}
