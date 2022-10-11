using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VFXController : MonoBehaviour
{
    public float MaxInterval;
    public float MinInterval;
    public AnimationCurve curve;
    public Color emissionColorBegin;
    public Color baseColorBegin;
    public Color emissionColorEnd;
    public Color baseColorEnd;
    public int gradientNum;

    protected Color curEColor;
    protected Color curBColor;
    protected Material mat;
    protected float timer;
    protected float lumin;
    protected static Color E_GRADIENT;
    protected static Color B_GRADIENT;
    protected static float I_GRADIENT;
    protected float interval;

    protected virtual void initializeVariables()
    {
        interval = MaxInterval;
        lumin = 2.0f;
        timer = 0.0f;
        E_GRADIENT = (emissionColorEnd - emissionColorBegin) / gradientNum;
        B_GRADIENT = (baseColorEnd - baseColorBegin) / gradientNum;
        I_GRADIENT = (MinInterval - MaxInterval) / gradientNum;
        mat = gameObject.GetComponent<Renderer>().material;
    }
}
