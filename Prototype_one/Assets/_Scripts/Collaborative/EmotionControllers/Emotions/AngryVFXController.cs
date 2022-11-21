using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryVFXController : VFXController
{
    public AnimationCurve colorGradient;

    public static float MAX_SPEED;
    void Start()
    {
        initializeVariables();
    }
    protected override void initializeVariables()
    {
        MAX_SPEED = 16f;
        base.initializeVariables();
        curEColor = emissionColorBegin;
        this.SetColor(gameObject.GetComponent<AngryAttribute>().GetSize());
    }
    public void SetColor(int size)
    {
        curEColor = Mathf.Floor(colorGradient.Evaluate(1.0f * size / gradientNum) * gradientNum) * E_GRADIENT + emissionColorBegin;
        /*interval = MaxInterval - Mathf.Floor(colorGradient.Evaluate(size / gradientNum)) * I_GRADIENT;*/
        /*mat.SetColor("_Color", curEColor * curve.Evaluate(timer / interval) * lumin);*/
        mat.SetColor("_Color", curEColor * 50);
        var curSpeed = mat.GetFloat("_TimeFactor");
        mat.SetFloat("_TimeFactor", MAX_SPEED/(1.0f *gradientNum / size));
    }
}
