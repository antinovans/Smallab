using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngerController : MonoBehaviour
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
    public GameObject AngerParticle;

    //rigidbody and transform related
    public static int DEFAULT_MASS = 2;
    public static Vector3 DEFAULT_SCALE = new Vector3(0.1f, 0.1f, 0.1f);
    public static int MAX_SIZE = 4;
    //vfx related
    public static float DEFAULT_TIME_FACTOR = 6.0f;
    public static float DEFAULT_CELL_DENSITY = 0.5f;
    public static float DEFAULT_CHAOS_FACTOR = 0.1f;
    //components on the gameobject
    //transform local fields
    private bool isScalingDown = false;
    //vfx local fields
    private Color curColor;
    private Material mat;
    // Start is called before the first frame update
    private void Start()
    {
        UpdateTransform();
        initializeVFXFields();
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
        var beginScale = prevSize * defaultScale;
        var endScale = size * defaultScale;
        while(timer < 1.0f)
        {
            timer += Time.deltaTime;
            float portion = scaleTransition.Evaluate(timer);
            transform.localScale = (1 - portion) * beginScale + portion * endScale;
            yield return null;
        }
        transform.localScale = endScale;
        isScalingDown = false;
    }
    IEnumerator LerpScale(float newSize)
    {
        float timer = 0.0f;
        var beginScale = prevSize * defaultScale;
        var endScale = newSize * defaultScale;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            float portion = scaleTransition.Evaluate(timer);
            transform.localScale = (1 - portion) * beginScale + portion * endScale;
            yield return null;
        }
        transform.localScale = endScale;
        isScalingDown = false;
        Destroy(gameObject);
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
        if (gameObject.CompareTag("BitterSweet"))
            return;

        if (collision.gameObject.CompareTag("Gate"))
        {
            collision.gameObject.GetComponent<GateVFXController>().HandleValue(this.size * this.defaultValue);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Joy") && !isScalingDown)
        {
            isScalingDown = true;
            SetSize(this.size / 2);
            ChangeColor();
        }
        if (collision.gameObject.CompareTag("Sadness") || collision.gameObject.CompareTag("Depression"))
        {
            isScalingDown = true;
            for(int i = 0; i < 6*size; i++)
            {
                var particle = Instantiate(AngerParticle, transform.position, Quaternion.identity);
                particle.GetComponent<BezierMovement>().SetTarget(collision.gameObject);
            }
            StartCoroutine(LerpScale(0f));
            Destroy(gameObject);
        }
    }
}
