using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadController : MonoBehaviour
{
    [Header("emotion transform Attributes")]
    //the size of the emotion
    public int size;
    private int prevSize = 1;
    //describe the positive value of the joy
    public int defaultValue;
    //default mass of the joy
    public int defaultMass;
    //default local scale of the joy
    public Vector3 defaultScale;
    //can the emotion split
    public bool isSplitable;
    //animation curve used to describe the transition speed
    [SerializeField]
    private AnimationCurve scaleTransition;

    [Header("emotion VFX Attributes")]
    public Color emissionColorBegin;
    public Color emissionColorEnd;
    public AnimationCurve colorGradient;
    public float intensity;
    public Color depressionColor;

    //rigidbody and transform related
    public static int DEFAULT_MASS = 2;
    public static Vector3 DEFAULT_SCALE = new Vector3(0.1f, 0.1f, 0.1f);
    public static int MAX_SIZE = 8;
    //vfx related
    public static float DEFAULT_TIME_FACTOR = 1.0f;
    public static float DEFAULT_CELL_DENSITY = 1.0f;
    public static float DEFAULT_CHAOS_FACTOR = 0f;
    //components on the gameobject
    //transform local fields
    private bool isScalingDown = false;
    //vfx local fields
    private Color curColor;
    private Material mat;
    //sad controller exclusive
    private bool isDepression = false;
    // Start is called before the first frame update
    private void Start()
    {
        UpdateTransform();
        initializeVFXFields();
    }

    private void Update()
    {
        if (!isDepression)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Anger");
            if (target != null)
                GetComponent<BasicMovement>().targetPos = target.transform;
        }
    }

    //transform related
    public void SetSize(int size)
    {
        this.prevSize = this.size;
        this.size = size;
        UpdateTransform();
    }
    private void UpdateTransform()
    {
        isSplitable = size > 1 ? true : false;
        if (!isSplitable)
        {
            gameObject.tag = "BitterSweet";
            this.defaultValue = 1;
        }
        StartCoroutine(LerpScale());
        gameObject.GetComponent<Rigidbody>().mass = size * defaultMass;
    }
    IEnumerator LerpScale()
    {
        float timer = 0.0f;
        /*while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            yield return null;
        }*/
        /*timer = 0.0f;*/
        var beginScale = prevSize * defaultScale;
        var endScale = size * defaultScale;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            float portion = scaleTransition.Evaluate(timer);
            transform.localScale = (1 - portion) * beginScale + portion * endScale;
            yield return null;
        }
        transform.localScale = endScale;
        isScalingDown = false;
    }

    //vfx related
    private void initializeVFXFields()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        curColor = emissionColorBegin;
        ChangeColor();
    }
    public void ChangeColor()
    {
        /*curColor = Mathf.Floor(colorGradient.Evaluate(this.size / MAX_SIZE) * gradientNum) * E_GRADIENT + emissionColorBegin;*/
        curColor = Color.Lerp(emissionColorBegin, emissionColorEnd, colorGradient.Evaluate((float)this.size / (float)MAX_SIZE));
        /*mat.SetColor("_Color", curEColor * 50f);*/
        StartCoroutine(LerpColor(curColor, 1f, intensity));
        var curSpeed = mat.GetFloat("_TimeFactor");
        mat.SetFloat("_TimeFactor", DEFAULT_TIME_FACTOR * this.size);
        mat.SetFloat("_CellDensity", DEFAULT_CELL_DENSITY * this.size);
        mat.SetFloat("_Chaos", DEFAULT_CHAOS_FACTOR * this.size);
    }
    IEnumerator LerpColor(Color c, float time, float intensity)
    {
        var initColor = mat.GetColor("_Color");
        float timer = 0.0f;
        while (timer <= time)
        {
            var tempColor = Color.Lerp(initColor, c, timer / time);
            mat.SetColor("_Color", tempColor * intensity);
            timer += Time.deltaTime;
            yield return null;
        }
        mat.SetColor("_Color", c * intensity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //sadness collision behavior
        if(!isDepression)
        {
            if (collision.gameObject.CompareTag("Joy") && isSplitable && !isScalingDown)
            {
                isScalingDown = true;
                SetSize(this.size / 2);
                ChangeColor();
            }
            if (collision.gameObject.CompareTag("Anger") && isSplitable)
            {
                isDepression = true;
                SetSize(collision.gameObject.GetComponent<AngerController>().size + this.size);
                TurnToDepression();
            }
            if (collision.gameObject.CompareTag("Depression") && isSplitable)
            {
                Destroy(collision.gameObject);
            }
        }
        //depression collision behavior
        if (isDepression)
        {
            if (collision.gameObject.CompareTag("Joy") && isSplitable && !isScalingDown)
            {
                isScalingDown = true;
                SetSize(this.size - collision.gameObject.GetComponent<JoyController>().size);
            }
            if (collision.gameObject.CompareTag("Anger") && !isScalingDown)
            {
                SetSize(collision.gameObject.GetComponent<AngerController>().size + this.size);
            }
            if (collision.gameObject.CompareTag("Sadness") && !isScalingDown)
            {
                SetSize(collision.gameObject.GetComponent<SadController>().size + this.size);
            }
        }

    }

    private void TurnToDepression()
    {
        gameObject.tag = "Depression";
        GameObject target = GameObject.FindGameObjectWithTag("Gate");
        if (target != null)
            GetComponent<BasicMovement>().targetPos = target.transform;
        StartCoroutine(LerpColor(depressionColor, 1f, intensity));
    }
}
