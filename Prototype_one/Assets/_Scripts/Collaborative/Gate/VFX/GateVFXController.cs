using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GateVFXController : VFXController
{
    public Color baseColorBegin;
    public Color baseColorEnd;

    protected static Color B_GRADIENT;
    protected int prevColorLevel;
    protected int colorLevel;
    // Start is called before the first frame update
    void Start()
    {
        initializeVariables();
    }

    protected override void initializeVariables()
    {
        base.initializeVariables();
        B_GRADIENT = (baseColorEnd - baseColorBegin) / gradientNum;
        prevColorLevel = 0;
        colorLevel = 0;
        curEColor = emissionColorBegin;
        curBColor = baseColorBegin;
        EventManager.current.onGateColorChange += GateColorResponse;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
    }
    void UpdateColor()
    {
        timer += Time.deltaTime;
        timer %= interval;
        mat.SetColor("_EmissionColor", curEColor * curve.Evaluate(timer / interval) * lumin);
        mat.SetColor("_BaseColor", curBColor);
    }
    void GateColorResponse(int input)
    {
        prevColorLevel = colorLevel;
        colorLevel += input;
        colorLevel = Mathf.Clamp(colorLevel, 0, gradientNum);
        if (colorLevel - prevColorLevel != 0)
        {
            curEColor += E_GRADIENT * (colorLevel - prevColorLevel);
            curBColor += B_GRADIENT * (colorLevel - prevColorLevel);
            interval += I_GRADIENT * (colorLevel - prevColorLevel);
            interval = Mathf.Clamp(interval, MinInterval, MaxInterval);
        }
    }

}
