using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GateVFXController : MonoBehaviour
{
    public float MaxInterval;
    public float MinInterval;
    public AnimationCurve curve;
    public UnityEvent colorEvent;
    public Color emissionColorBegin;
    public Color baseColorBegin;
    public Color emissionColorEnd;
    public Color baseColorEnd;
    public int gradientNum;

    private Color curEColor;
    private Color curBColor;
    private int colorLevel;
    private int prevColorLevel;
    private Material mat;
    private float timer;
    private float lumin;
    private static Color BEGINNING_COLOR;
    private static Color ENDING_COLOR;
    private static Color E_GRADIENT;
    private static Color B_GRADIENT;
    private static float I_GRADIENT;
    private float interval;
    // Start is called before the first frame update
    void Start()
    {
        initializeParameters();
        mat = gameObject.GetComponent<Renderer>().material;
        curEColor = emissionColorBegin;
        curBColor = baseColorBegin;
        colorEvent = new UnityEvent();
        colorEvent.AddListener(ColorAction);
    }

    private void initializeParameters()
    {
        interval = MaxInterval;
        lumin = 2.0f;
        timer = 0.0f;
        colorLevel = 0;
        prevColorLevel = 0;
        E_GRADIENT = (emissionColorEnd - emissionColorBegin) / gradientNum;
        B_GRADIENT = (baseColorEnd - baseColorBegin) / gradientNum;
        I_GRADIENT = (MinInterval - MaxInterval) / gradientNum; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && colorEvent != null)
        {
            //Begin the action
            colorEvent.Invoke();
        }
        timer += Time.deltaTime;
        timer %= interval;
        mat.SetColor("_EmissionColor", curEColor * curve.Evaluate(timer/interval) * lumin);
        mat.SetColor("_BaseColor", curBColor);
    }

    void ColorAction()
    {
        prevColorLevel = colorLevel;
        colorLevel += 1;
        colorLevel = Mathf.Clamp(colorLevel, 0, gradientNum);
        if(colorLevel - prevColorLevel != 0) {
            curEColor += E_GRADIENT * (colorLevel - prevColorLevel);
            curBColor += B_GRADIENT * (colorLevel - prevColorLevel);
            interval += I_GRADIENT * (colorLevel - prevColorLevel);
            interval = Mathf.Clamp(interval, MinInterval, MaxInterval);
            Debug.Log(curEColor);
        }
    }

}
