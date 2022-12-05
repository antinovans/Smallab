using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GateVFXController :MonoBehaviour
{
    [Header("Material stats")]
    public Color baseColorBegin;
    public Color baseColorEnd;
    public float intervalBegin;
    public float intervalEnd;
    public AnimationCurve evaluateCurve;

    public int maxHealth;

    private int prevHealth;
    private int curHealth;
    private Renderer[] children;
    // Start is called before the first frame update
    void Start()
    {
        children = GetComponentsInChildren<Renderer>();
        Debug.Log(children.Length);
        foreach (var r in children)
        {
            r.material.SetFloat("_Timer", intervalBegin);
            r.material.SetColor("_EmissionColor", baseColorBegin);
        }
        curHealth = maxHealth;
        prevHealth = curHealth;
    }

    public void HandleValue(int value)
    {
        prevHealth = curHealth;
        curHealth = Mathf.Clamp(curHealth + value, 0, maxHealth);
        Debug.Log("testing int to float " + ((float)curHealth / (float)maxHealth));
        float portion = evaluateCurve.Evaluate(1.0f - ((float)curHealth / (float)maxHealth));
        Debug.Log("portion is " + portion);
        float targetInterval = (1- portion)*intervalBegin + portion * intervalEnd;
        Debug.Log("targetInterval is " + targetInterval);
        var targetEmissionColor = Color.Lerp(baseColorBegin, baseColorEnd, portion);
        foreach (var r in children)
        {
            StartCoroutine(LerpFloat(r, "_Timer", r.material.GetFloat("_Timer"), targetInterval, 1f));
            StartCoroutine(LerpColor(r, "_EmissionColor", r.material.GetColor("_EmissionColor"), targetEmissionColor, 1f));
        }

    }
    IEnumerator LerpFloat(Renderer r, string attributeName, float begin, float end, float time)
    {
        float timer = 0.0f;
        while (timer <= time)
        {
            float portion = timer / time;
            r.material.SetFloat(attributeName, (1 - portion) * begin + portion * end);
            timer += Time.deltaTime;
            yield return null;
        }
        r.material.SetFloat(attributeName, end);
    }
    IEnumerator LerpColor(Renderer r, string colorName, Color begin, Color end, float time)
    {
        var initColor = r.material.GetColor(colorName);
        float timer = 0.0f;
        while (timer <= time)
        {
            var tempColor = Color.Lerp(begin, end, timer / time);
            r.material.SetColor(colorName, tempColor);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Joy"))
        {
            Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.CompareTag("Anger"))
        {
            Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.CompareTag("Sadness"))
        {
            Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.CompareTag("Depression"))
        {
            Destroy(collision.gameObject);
            return;
        }
    }

}
