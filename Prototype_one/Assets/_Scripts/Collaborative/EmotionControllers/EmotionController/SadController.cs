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
    [Header("curves")]
    public AnimationCurve colorGradient;
    public AnimationCurve scaleTransition;

    //rigidbody and transform related
    [Header("sadness VFX Attributes")]
    public Color glowColorStart;
    public Color glowColorEnd;
    public Color primaryColor;

    /*public static float DEFAULT_TIME_FACTOR = 1.0f;
    public static float DEFAULT_CELL_DENSITY = 1.0f;
    public static float DEFAULT_CHAOS_FACTOR = 0f;*/
    [Header("depression VFX Attributes")]
    public Color glowColor;
    public Color primaryColorStart;
    public Color primaryColorEnd;
    public float noiseScaleStart;
    public float noiseScaleEnd;

    [SerializeField]
    private GameObject sadnessParticle;

    public static int MAX_SIZE = 4;
    //components on the gameobject
    //transform local fields
    private bool isScalingDown = false;
    private Material mat;
    //sad controller exclusive
    private bool isDepression = false;

    private GameObject target = null;
    // Start is called before the first frame update
    private void Start()
    {
        initializeSadness();
    }

    private void initializeSadness()
    {
        isSplitable = size > 1 ? true : false;
        if (!isSplitable)
        {
            gameObject.tag = "BitterSweet";
            this.defaultValue = 1;
        }
        gameObject.GetComponent<Rigidbody>().mass = size * defaultMass;
        transform.localScale = size * defaultScale;

        mat = gameObject.GetComponent<Renderer>().material;
        var curGlowColor = Color.Lerp(glowColorStart, glowColorEnd, Mathf.Clamp01(colorGradient.Evaluate((float)this.size / (float)MAX_SIZE)));
        mat.SetColor("_GlowColor", curGlowColor);
        mat.SetColor("_PrimaryColor", primaryColor);
        mat.SetFloat("_NoiseScale", 20f);


    }

    private void Update()
    {
        if (!isDepression && target == null)
        {
            target = GameObject.FindGameObjectWithTag("Anger");
            if (target != null)
                GetComponent<BasicMovement>().targetPos = target.transform;
        }
    }

    //transform related
    public void SetSize(int size)
    {
        this.prevSize = this.size;
        this.size = size;
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

    public void sadnessChangeColor()
    {
        var curGlowColor = Color.Lerp(glowColorStart, glowColorEnd, Mathf.Clamp01(colorGradient.Evaluate((float)this.size / (float)MAX_SIZE)));
        StartCoroutine(LerpColor("_GlowColor", curGlowColor, 1f, 1f));
    }
    public void depressionChangeColor()
    {
        StartCoroutine(LerpColor("_GlowColor", glowColor, 1f, 1f));
        var curPrimaryColor = Color.Lerp(primaryColorStart, primaryColorEnd, Mathf.Clamp01(colorGradient.Evaluate((float)this.size / (float)MAX_SIZE)));
        StartCoroutine(LerpColor("_PrimaryColor", curPrimaryColor, 1f, 1f));
        float portion = Mathf.Clamp01(colorGradient.Evaluate((float)this.size / (float)MAX_SIZE));
        float noiseScale = noiseScaleStart + portion*(noiseScaleEnd - noiseScaleStart);
        Debug.Log("NoiseSacle: " + noiseScale);
        StartCoroutine(LerpFloat("_NoiseScale", mat.GetFloat("_NoiseScale"), noiseScale, 1.0f));

    }

    IEnumerator LerpFloat(string attributeName, float begin, float end, float time)
    {
        float timer = 0.0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            float portion = colorGradient.Evaluate(timer);
            mat.SetFloat(attributeName, (1 - portion) * begin + portion * end);
            yield return null;
        }
        mat.SetFloat(attributeName, end);
    }
    IEnumerator LerpColor(string colorName, Color c, float time, float intensity)
    {
        var initColor = mat.GetColor(colorName);
        float timer = 0.0f;
        while (timer <= time)
        {
            var tempColor = Color.Lerp(initColor, c, timer / time);
            mat.SetColor(colorName, tempColor * intensity);
            timer += Time.deltaTime;
            yield return null;
        }
        mat.SetColor(colorName, c * intensity);
    }

    private void TurnToDepression()
    {
        gameObject.tag = "Depression";
        GameObject target = GameObject.FindGameObjectWithTag("Gate");
        if (target != null)
            GetComponent<BasicMovement>().targetPos = target.transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("BitterSweet"))
            return;
        if (collision.gameObject.CompareTag("Gate"))
        {
            for (int i = 0; i < 6 * size; i++)
            {
                var particle = Instantiate(sadnessParticle, transform.position, Quaternion.identity);
                particle.GetComponent<BezierMovement>().SetTarget(collision.gameObject);
            }
            Destroy(gameObject);
            collision.gameObject.GetComponent<GateVFXController>().HandleValue(this.size * this.defaultValue);
        }
        //sadness collision behavior
        if (!isDepression)
        {
            if (collision.gameObject.CompareTag("Joy") && !isScalingDown)
            {
                isScalingDown = true;
                SetSize(this.size / 2);
                sadnessChangeColor();
            }
            if (collision.gameObject.CompareTag("Anger") && !isScalingDown)
            {
                isScalingDown = true;
                isDepression = true;
                SetSize(collision.gameObject.GetComponent<AngerController>().size + this.size);
                TurnToDepression();
                depressionChangeColor();
            }
            if (collision.gameObject.CompareTag("Depression"))
            {
                for (int i = 0; i < 6 * size; i++)
                {
                    var particle = Instantiate(sadnessParticle, transform.position, Quaternion.identity);
                    particle.GetComponent<BezierMovement>().SetTarget(collision.gameObject);
                }
                Destroy(gameObject);
            }
        }
        //depression collision behavior
        if (isDepression)
        {
            if (collision.gameObject.CompareTag("Joy") && !isScalingDown)
            {
                isScalingDown = true;
                SetSize(this.size - collision.gameObject.GetComponent<JoyController>().size);
                depressionChangeColor();
            }
            if (collision.gameObject.CompareTag("Anger") && !isScalingDown)
            {
                SetSize(collision.gameObject.GetComponent<AngerController>().size + this.size);
                depressionChangeColor();
            }
            if (collision.gameObject.CompareTag("Sadness") && !isScalingDown)
            {
                SetSize(collision.gameObject.GetComponent<SadController>().size + this.size);
                depressionChangeColor();
            }
        }

    }
}
