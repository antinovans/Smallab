using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryVFXController : VFXController
{
    public bool test;
    public AnimationCurve colorGradient;
    void Start()
    {
        initializeVariables();
    }
    protected override void initializeVariables()
    {
        base.initializeVariables();
        curEColor = emissionColorBegin;
        curBColor = baseColorBegin;
        this.SetColor(gameObject.GetComponent<AngryAttribute>().GetSize());
    }
    private void Update()
    {
        if (test)
        {
            SetColor(4);
        }
    }
    /*    void Update()
        {
            UpdateColor();
        }*/
    /*    void UpdateColor()
        {
            timer += Time.deltaTime;
            timer %= interval;
            mat.SetColor("_EmissionColor", curEColor * curve.Evaluate(timer / interval) * lumin);
            mat.SetColor("_BaseColor", curBColor);
        }*/
    public void SetColor(int size)
    {
        curEColor = Mathf.Floor(colorGradient.Evaluate(1.0f * size / gradientNum) * gradientNum) * E_GRADIENT + emissionColorBegin;
        curBColor = Mathf.Floor(colorGradient.Evaluate(1.0f * size / gradientNum) * gradientNum) * B_GRADIENT + baseColorBegin;
        /*interval = MaxInterval - Mathf.Floor(colorGradient.Evaluate(size / gradientNum)) * I_GRADIENT;*/
        mat.SetColor("_EmissionColor", curEColor * curve.Evaluate(timer / interval) * lumin);
        mat.SetColor("_BaseColor", curBColor);
    }
}
